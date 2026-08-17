using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using src.db;
using static src.services.ServiceMap;

namespace src.services
{
    public static class LecturerMaterialReader
    {
        public static void EnsureAssessmentAssignments()
        {
            EnsureMaterialColumns();
        }

        public static LecturerMaterialRow GetMaterial(UserContext user, int materialId)
        {
            if (user == null || materialId <= 0) return null;
            EnsureMaterialColumns();

            string sql =
                "SELECT mat.material_id, md.offer_id, mat.title, mat.description, mat.file_url, " +
                "mat.file_type, mat.file_size_bytes, mat.material_type, mat.due_date, mat.weight, mat.uploaded_at, md.week_number, " +
                "c.course_code, c.course_name, co.academic_year, co.semester " +
                "FROM MATERIALS mat " +
                "JOIN MODULES md ON md.module_id = mat.module_id " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = md.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "WHERE mat.material_id = @materialId AND " + ServiceAccess.VisibleOfferScope("co");

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                cmd.Parameters.AddWithValue("@materialId", materialId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new LecturerMaterialRow
                    {
                        MaterialId = IntValue(reader["material_id"]),
                        OfferingId = IntValue(reader["offer_id"]),
                        CourseCode = Text(reader["course_code"]),
                        CourseName = Text(reader["course_name"]),
                        AcademicYear = Text(reader["academic_year"]),
                        Semester = Text(reader["semester"]),
                        Title = Text(reader["title"]),
                        Description = Text(reader["description"]),
                        FileUrl = Text(reader["file_url"]),
                        FileType = Text(reader["file_type"]),
                        FileSizeBytes = NullableInt(reader["file_size_bytes"]),
                        MaterialType = Text(reader["material_type"]),
                        Week = NullableInt(reader["week_number"]),
                        DueDate = DateValue(reader["due_date"]),
                        Weight = DecimalValue(reader["weight"]),
                        UploadedAt = DateValue(reader["uploaded_at"]) ?? DateTime.Now
                    };
                }
            }
        }

        public static decimal GetWeightTotal(UserContext user, int offeringId)
        {
            if (user == null || offeringId <= 0) return 0m;
            EnsureMaterialColumns();

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageOffer(conn, user, offeringId)) return 0m;
                using (var cmd = new SqlCommand(
                    "SELECT ISNULL(SUM(mat.weight), 0) " +
                    "FROM MATERIALS mat JOIN MODULES md ON md.module_id = mat.module_id " +
                    "WHERE md.offer_id = @offerId", conn))
                {
                    cmd.Parameters.AddWithValue("@offerId", offeringId);
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
        }

        public static List<LecturerMaterialRow> GetMaterials(UserContext user, int? offeringId)
        {
            var rows = new List<LecturerMaterialRow>();
            if (user == null || !user.IsLecturer) return rows;
            EnsureMaterialColumns();

            string sql =
                "SELECT mat.material_id, md.offer_id, mat.title, mat.description, mat.file_url, " +
                "mat.file_type, mat.file_size_bytes, mat.material_type, mat.due_date, mat.weight, mat.uploaded_at, md.week_number, " +
                "c.course_code, c.course_name, co.academic_year, co.semester " +
                "FROM MATERIALS mat " +
                "JOIN MODULES md ON md.module_id = mat.module_id " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = md.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "WHERE " + ServiceAccess.VisibleOfferScope("co") + " " +
                "AND (@offerId = 0 OR md.offer_id = @offerId) " +
                "ORDER BY mat.uploaded_at DESC, mat.material_id DESC";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                cmd.Parameters.AddWithValue("@offerId", offeringId.GetValueOrDefault());
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(new LecturerMaterialRow
                        {
                            MaterialId = IntValue(reader["material_id"]),
                            OfferingId = IntValue(reader["offer_id"]),
                            CourseCode = Text(reader["course_code"]),
                            CourseName = Text(reader["course_name"]),
                            AcademicYear = Text(reader["academic_year"]),
                            Semester = Text(reader["semester"]),
                            Title = Text(reader["title"]),
                            Description = Text(reader["description"]),
                            FileUrl = Text(reader["file_url"]),
                            UploadedAt = DateValue(reader["uploaded_at"]) ?? DateTime.Now,
                            FileType = Text(reader["file_type"]),
                            FileSizeBytes = NullableInt(reader["file_size_bytes"]),
                            MaterialType = Text(reader["material_type"]),
                            Week = NullableInt(reader["week_number"]),
                            DueDate = DateValue(reader["due_date"]),
                            Weight = DecimalValue(reader["weight"])
                        });
                    }
                }
            }
            return rows;
        }

        // Returns the new material id, 0 for a general failure, or -1 when
        // adding the requested weight would exceed the course's 100% total.
        public static int Add(UserContext user, LecturerMaterialInput input)
        {
            if (user == null || input == null || input.OfferingId <= 0) return 0;
            EnsureMaterialColumns();

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageOffer(conn, user, input.OfferingId)) return 0;

                using (var transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    string normalizedType = NormalizeMaterialType(input.MaterialType);
                    if (normalizedType == "Lecture Notes" &&
                        (!input.Week.HasValue || input.Week.Value < 1 || input.Week.Value > 14))
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    decimal existingWeight;
                    using (var weightCommand = new SqlCommand(
                        "SELECT ISNULL(SUM(mat.weight), 0) " +
                        "FROM MATERIALS mat WITH (UPDLOCK, HOLDLOCK) " +
                        "JOIN MODULES md ON md.module_id = mat.module_id " +
                        "WHERE md.offer_id = @offerId", conn, transaction))
                    {
                        weightCommand.Parameters.AddWithValue("@offerId", input.OfferingId);
                        existingWeight = Convert.ToDecimal(weightCommand.ExecuteScalar());
                    }

                    decimal requestedWeight = input.Weight.GetValueOrDefault();
                    if (existingWeight + requestedWeight > 100m)
                    {
                        transaction.Rollback();
                        return -1;
                    }

                    string moduleId;
                    if (normalizedType == "Lecture Notes" && input.Week.HasValue)
                    {
                        using (var moduleCommand = new SqlCommand(
                            "SELECT TOP 1 module_id FROM MODULES WITH (UPDLOCK, HOLDLOCK) " +
                            "WHERE offer_id = @offerId AND week_number = @week ORDER BY module_id",
                            conn, transaction))
                        {
                            moduleCommand.Parameters.AddWithValue("@offerId", input.OfferingId);
                            moduleCommand.Parameters.AddWithValue("@week", input.Week.Value);
                            var value = moduleCommand.ExecuteScalar();
                            moduleId = value == null || value == DBNull.Value ? "" : value.ToString();
                        }

                        if (string.IsNullOrWhiteSpace(moduleId))
                        {
                            moduleId = "MOD_" + input.OfferingId + "_W" + input.Week.Value;
                            using (var createModule = new SqlCommand(
                                "INSERT INTO MODULES (module_id, offer_id, module_title, module_description, week_number) " +
                                "VALUES (@moduleId, @offerId, @title, @description, @week)",
                                conn, transaction))
                            {
                                createModule.Parameters.AddWithValue("@moduleId", moduleId);
                                createModule.Parameters.AddWithValue("@offerId", input.OfferingId);
                                createModule.Parameters.AddWithValue("@title", "Week " + input.Week.Value);
                                createModule.Parameters.AddWithValue("@description", "Lecture materials for Week " + input.Week.Value + ".");
                                createModule.Parameters.AddWithValue("@week", input.Week.Value);
                                createModule.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        using (var moduleCommand = new SqlCommand(
                            "SELECT TOP 1 module_id FROM MODULES WITH (UPDLOCK, HOLDLOCK) " +
                            "WHERE offer_id = @offerId ORDER BY week_number, module_id",
                            conn, transaction))
                        {
                            moduleCommand.Parameters.AddWithValue("@offerId", input.OfferingId);
                            var value = moduleCommand.ExecuteScalar();
                            moduleId = value == null || value == DBNull.Value ? "" : value.ToString();
                        }

                        if (string.IsNullOrWhiteSpace(moduleId))
                        {
                            moduleId = "MOD_" + input.OfferingId + "_GEN";
                            using (var createModule = new SqlCommand(
                                "INSERT INTO MODULES (module_id, offer_id, module_title, module_description, week_number) " +
                                "VALUES (@moduleId, @offerId, @title, @description, NULL)",
                                conn, transaction))
                            {
                                createModule.Parameters.AddWithValue("@moduleId", moduleId);
                                createModule.Parameters.AddWithValue("@offerId", input.OfferingId);
                                createModule.Parameters.AddWithValue("@title", "General");
                                createModule.Parameters.AddWithValue("@description", "General course materials.");
                                createModule.ExecuteNonQuery();
                            }
                        }
                    }

                    int? assignmentId = null;
                    if (normalizedType == "Assignment" || normalizedType == "Quiz" ||
                        normalizedType == "Test" || normalizedType == "Viva")
                    {
                        string submissionMode = normalizedType == "Assignment"
                            ? "FILE"
                            : normalizedType == "Viva" &&
                              string.Equals(input.SubmissionMode, "LINK", StringComparison.OrdinalIgnoreCase)
                                ? "LINK"
                                : "MANUAL";
                        const string assignmentSql =
                            "INSERT INTO ASSIGNMENTS (offer_id, title, description, file_path, total_marks, due_date, submission_mode) " +
                            "OUTPUT INSERTED.assignment_id " +
                            "VALUES (@offerId, @title, @description, @filePath, 100, @dueDate, @submissionMode)";
                        using (var assignmentCommand = new SqlCommand(assignmentSql, conn, transaction))
                        {
                            assignmentCommand.Parameters.AddWithValue("@offerId", input.OfferingId);
                            assignmentCommand.Parameters.AddWithValue("@title", ServiceAccess.DbValue(input.Title));
                            assignmentCommand.Parameters.AddWithValue("@description", ServiceAccess.DbValue(input.Description));
                            assignmentCommand.Parameters.AddWithValue("@filePath", ServiceAccess.DbValue(input.FileUrl));
                            assignmentCommand.Parameters.AddWithValue("@dueDate", ServiceAccess.DbValue(input.DueDate));
                            assignmentCommand.Parameters.AddWithValue("@submissionMode", submissionMode);
                            assignmentId = Convert.ToInt32(assignmentCommand.ExecuteScalar());
                        }
                    }

                    const string sql =
                        "INSERT INTO MATERIALS (module_id, title, description, file_url, file_type, file_size_bytes, " +
                        "material_type, due_date, weight, uploaded_at, assignment_id) " +
                        "OUTPUT INSERTED.material_id " +
                        "VALUES (@moduleId, @title, @description, @fileUrl, @fileType, @fileSizeBytes, " +
                        "@materialType, @dueDate, @weight, @uploadedAt, @assignmentId)";
                    using (var cmd = new SqlCommand(sql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@moduleId", moduleId);
                        cmd.Parameters.AddWithValue("@title", ServiceAccess.DbValue(input.Title));
                        cmd.Parameters.AddWithValue("@description", ServiceAccess.DbValue(input.Description));
                        cmd.Parameters.AddWithValue("@fileUrl", ServiceAccess.DbValue(input.FileUrl));
                        cmd.Parameters.AddWithValue("@fileType", ServiceAccess.DbValue(input.FileType));
                        cmd.Parameters.AddWithValue("@fileSizeBytes", ServiceAccess.DbValue(input.FileSizeBytes));
                        cmd.Parameters.AddWithValue("@materialType", normalizedType);
                        cmd.Parameters.AddWithValue("@dueDate", ServiceAccess.DbValue(input.DueDate));
                        var weightParameter = cmd.Parameters.Add("@weight", SqlDbType.Decimal);
                        weightParameter.Precision = 5;
                        weightParameter.Scale = 2;
                        weightParameter.Value = ServiceAccess.DbValue(input.Weight);
                        cmd.Parameters.AddWithValue("@uploadedAt", input.UploadedAt == default(DateTime) ? DateTime.Now : input.UploadedAt);
                        cmd.Parameters.AddWithValue("@assignmentId", ServiceAccess.DbValue(assignmentId));
                        var id = cmd.ExecuteScalar();
                        transaction.Commit();
                        return id == null || id == DBNull.Value ? 0 : Convert.ToInt32(id);
                    }
                }
            }
        }

        public static bool Delete(UserContext user, int materialId)
        {
            if (user == null) return false;
            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageMaterial(conn, user, materialId)) return false;
                using (var cmd = new SqlCommand("DELETE FROM MATERIALS WHERE material_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", materialId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static MaterialWeightUpdateResult UpdateWeight(UserContext user, int materialId, decimal weight)
        {
            var result = new MaterialWeightUpdateResult
            {
                Success = false,
                Message = "Course weight could not be updated."
            };
            if (user == null || materialId <= 0) return result;

            weight = Math.Round(weight, 2);
            if (weight <= 0m || weight > 100m)
            {
                result.Message = "Course weight must be greater than 0% and no more than 100%.";
                return result;
            }

            EnsureMaterialColumns();
            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageMaterial(conn, user, materialId))
                {
                    result.Message = "You do not have permission to edit this assessment.";
                    return result;
                }

                using (var transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    int offerId;
                    using (var lookup = new SqlCommand(
                        "SELECT md.offer_id FROM MATERIALS mat WITH (UPDLOCK, HOLDLOCK) " +
                        "JOIN MODULES md ON md.module_id = mat.module_id " +
                        "WHERE mat.material_id = @materialId AND mat.assignment_id IS NOT NULL",
                        conn, transaction))
                    {
                        lookup.Parameters.AddWithValue("@materialId", materialId);
                        var value = lookup.ExecuteScalar();
                        if (value == null || value == DBNull.Value)
                        {
                            result.Message = "Only published assessments have editable course weight.";
                            return result;
                        }
                        offerId = Convert.ToInt32(value);
                    }

                    decimal otherWeight;
                    using (var total = new SqlCommand(
                        "SELECT ISNULL(SUM(mat.weight), 0) FROM MATERIALS mat WITH (UPDLOCK, HOLDLOCK) " +
                        "JOIN MODULES md ON md.module_id = mat.module_id " +
                        "WHERE md.offer_id = @offerId AND mat.material_id <> @materialId",
                        conn, transaction))
                    {
                        total.Parameters.AddWithValue("@offerId", offerId);
                        total.Parameters.AddWithValue("@materialId", materialId);
                        otherWeight = Convert.ToDecimal(total.ExecuteScalar());
                    }

                    decimal maximum = Math.Max(0m, 100m - otherWeight);
                    if (weight > maximum)
                    {
                        result.Message = "Other assessments already use " +
                            otherWeight.ToString("0.##") + "%. The maximum available weight is " +
                            maximum.ToString("0.##") + "%.";
                        return result;
                    }

                    using (var update = new SqlCommand(
                        "UPDATE MATERIALS SET weight = @weight WHERE material_id = @materialId",
                        conn, transaction))
                    {
                        var parameter = update.Parameters.Add("@weight", SqlDbType.Decimal);
                        parameter.Precision = 5;
                        parameter.Scale = 2;
                        parameter.Value = weight;
                        update.Parameters.AddWithValue("@materialId", materialId);
                        if (update.ExecuteNonQuery() != 1) return result;
                    }

                    transaction.Commit();
                    result.Success = true;
                    result.Weight = weight;
                    result.CourseTotal = otherWeight + weight;
                    result.Message = "Course weight updated to " + weight.ToString("0.##") +
                        "%. Existing submissions were preserved.";
                    return result;
                }
            }
        }

        public static bool UpdateModule(UserContext user, int offeringId, string moduleId, string title, string description)
        {
            if (user == null || offeringId <= 0 || string.IsNullOrWhiteSpace(moduleId) ||
                string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
                return false;

            title = title.Trim();
            description = description.Trim();
            if (title.Length > 100 || description.Length > 255) return false;

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageOffer(conn, user, offeringId) ||
                    !ServiceAccess.CanManageModule(conn, user, moduleId))
                    return false;

                const string sql =
                    "UPDATE MODULES SET module_title = @title, module_description = @description " +
                    "WHERE module_id = @moduleId AND offer_id = @offerId";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@moduleId", moduleId.Trim());
                    cmd.Parameters.AddWithValue("@offerId", offeringId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private static void EnsureMaterialColumns()
        {
            const string schemaSql =
                "IF COL_LENGTH('MATERIALS', 'description') IS NULL ALTER TABLE MATERIALS ADD description varchar(500) NULL; " +
                "IF COL_LENGTH('MATERIALS', 'file_type') IS NULL ALTER TABLE MATERIALS ADD file_type varchar(20) NULL; " +
                "IF COL_LENGTH('MATERIALS', 'file_size_bytes') IS NULL ALTER TABLE MATERIALS ADD file_size_bytes int NULL; " +
                "IF COL_LENGTH('MATERIALS', 'material_type') IS NULL ALTER TABLE MATERIALS ADD material_type varchar(30) NULL; " +
                "IF COL_LENGTH('MATERIALS', 'due_date') IS NULL ALTER TABLE MATERIALS ADD due_date date NULL; " +
                "IF COL_LENGTH('MATERIALS', 'weight') IS NULL ALTER TABLE MATERIALS ADD weight decimal(5,2) NULL; " +
                "IF COL_LENGTH('MATERIALS', 'assignment_id') IS NULL ALTER TABLE MATERIALS ADD assignment_id int NULL; " +
                "IF COL_LENGTH('ASSIGNMENTS', 'submission_mode') IS NULL ALTER TABLE ASSIGNMENTS ADD submission_mode varchar(20) NULL";

            const string dataSql =
                "UPDATE MATERIALS SET material_type = CASE " +
                "WHEN LOWER(title) LIKE '%assignment%' THEN 'Assignment' " +
                "WHEN LOWER(title) LIKE '%quiz%' THEN 'Quiz' " +
                "WHEN LOWER(title) LIKE '%test%' OR LOWER(title) LIKE '%exam%' THEN 'Test' " +
                "ELSE 'Lecture Notes' END " +
                "WHERE material_type IS NULL OR LTRIM(RTRIM(material_type)) = ''";

            using (var conn = Db.OpenConnection())
            {
                // SQL Server compiles a batch before executing it, so an UPDATE
                // cannot reference a column added earlier in that same batch.
                using (var schemaCommand = new SqlCommand(schemaSql, conn))
                {
                    schemaCommand.ExecuteNonQuery();
                }
                using (var dataCommand = new SqlCommand(dataSql, conn))
                {
                    dataCommand.ExecuteNonQuery();
                }
                using (var modeCommand = new SqlCommand(
                    "UPDATE a SET submission_mode = CASE WHEN mat.material_type IN ('Quiz','Test') THEN 'MANUAL' " +
                    "WHEN mat.material_type = 'Viva' THEN 'MANUAL' ELSE 'FILE' END " +
                    "FROM ASSIGNMENTS a LEFT JOIN MATERIALS mat ON mat.assignment_id = a.assignment_id " +
                    "WHERE a.submission_mode IS NULL OR LTRIM(RTRIM(a.submission_mode)) = ''; " +
                    "UPDATE sub SET status = 'AWAITING_MARKS', marks_obtained = NULL " +
                    "FROM SUBMISSIONS sub JOIN ASSIGNMENTS a ON a.assignment_id = sub.assignment_id " +
                    "WHERE a.submission_mode = 'MANUAL' AND sub.status = 'MISSING' AND sub.file_url IS NULL", conn))
                {
                    modeCommand.ExecuteNonQuery();
                }
                BackfillAssessmentAssignments(conn);
            }
        }

        private static void BackfillAssessmentAssignments(SqlConnection conn)
        {
            const string selectSql =
                "SELECT mat.material_id, md.offer_id, mat.title, mat.description, mat.file_url, mat.due_date, mat.material_type " +
                "FROM MATERIALS mat JOIN MODULES md ON md.module_id = mat.module_id " +
                "WHERE mat.assignment_id IS NULL AND mat.material_type IN ('Assignment', 'Quiz', 'Test', 'Viva')";

            var pending = new List<Tuple<int, int, string, string, string, DateTime?, string>>();
            using (var select = new SqlCommand(selectSql, conn))
            using (var reader = select.ExecuteReader())
            {
                while (reader.Read())
                {
                    pending.Add(Tuple.Create(
                        IntValue(reader["material_id"]),
                        IntValue(reader["offer_id"]),
                        Text(reader["title"]),
                        Text(reader["description"]),
                        Text(reader["file_url"]),
                        DateValue(reader["due_date"]),
                        Text(reader["material_type"])));
                }
            }

            foreach (var material in pending)
            {
                using (var transaction = conn.BeginTransaction())
                {
                    const string insertSql =
                        "INSERT INTO ASSIGNMENTS (offer_id, title, description, file_path, total_marks, due_date, submission_mode) " +
                        "OUTPUT INSERTED.assignment_id " +
                        "VALUES (@offerId, @title, @description, @filePath, 100, @dueDate, @submissionMode)";
                    int assignmentId;
                    using (var insert = new SqlCommand(insertSql, conn, transaction))
                    {
                        insert.Parameters.AddWithValue("@offerId", material.Item2);
                        insert.Parameters.AddWithValue("@title", ServiceAccess.DbValue(material.Item3));
                        insert.Parameters.AddWithValue("@description", ServiceAccess.DbValue(material.Item4));
                        insert.Parameters.AddWithValue("@filePath", ServiceAccess.DbValue(material.Item5));
                        insert.Parameters.AddWithValue("@dueDate", ServiceAccess.DbValue(material.Item6));
                        insert.Parameters.AddWithValue("@submissionMode",
                            string.Equals(material.Item7, "Assignment", StringComparison.OrdinalIgnoreCase)
                                ? "FILE" : "MANUAL");
                        assignmentId = Convert.ToInt32(insert.ExecuteScalar());
                    }

                    using (var update = new SqlCommand(
                        "UPDATE MATERIALS SET assignment_id = @assignmentId " +
                        "WHERE material_id = @materialId AND assignment_id IS NULL", conn, transaction))
                    {
                        update.Parameters.AddWithValue("@assignmentId", assignmentId);
                        update.Parameters.AddWithValue("@materialId", material.Item1);
                        if (update.ExecuteNonQuery() == 0)
                        {
                            using (var cleanup = new SqlCommand(
                                "DELETE FROM ASSIGNMENTS WHERE assignment_id = @assignmentId", conn, transaction))
                            {
                                cleanup.Parameters.AddWithValue("@assignmentId", assignmentId);
                                cleanup.ExecuteNonQuery();
                            }
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        private static string NormalizeMaterialType(string materialType)
        {
            if (string.Equals(materialType, "Assignment", StringComparison.OrdinalIgnoreCase)) return "Assignment";
            if (string.Equals(materialType, "Quiz", StringComparison.OrdinalIgnoreCase)) return "Quiz";
            if (string.Equals(materialType, "Test", StringComparison.OrdinalIgnoreCase)) return "Test";
            if (string.Equals(materialType, "Viva", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(materialType, "Viva / Presentation", StringComparison.OrdinalIgnoreCase)) return "Viva";
            return "Lecture Notes";
        }

        private static int? NullableInt(object value)
        {
            return value == null || value == DBNull.Value ? (int?)null : Convert.ToInt32(value);
        }
    }
}
