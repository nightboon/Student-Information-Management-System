namespace src.services.email
{
    /// <summary>
    /// A fully rendered email, produced by an EmailService builder and handed to
    /// the private transport. PdfPath is optional (null = no attachment).
    /// </summary>
    public class EmailMessage
    {
        public string Subject { get; set; }
        public string Html { get; set; }
        public string Text { get; set; }    // plain-text fallback
        public string PdfPath { get; set; }  // optional file path to attach; null = none
    }
}
