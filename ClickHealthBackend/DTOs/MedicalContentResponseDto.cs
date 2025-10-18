namespace ClickHealthBackend.DTOs
{
    // File: DTOs/MedicalContentResponseDto.cs
    public class MedicalContentResponseDto
    {
        public string MedicalName { get; set; }
        public string ContentLanguage { get; set; }
        public string Description { get; set; }
        public string ViewPdf { get; set; }
        public string ViewVideo { get; set; }
        public DateTime? EndDate { get; set; }
        public string Actions { get; set; }  // For frontend buttons (Edit/Delete/View)
    }
    
}
