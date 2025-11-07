using SQLite;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DoctorId { get; set; }

    // Patient personal information
    public string Address { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string EmergencyContactName { get; set; }
    public string EmergencyContactPhone { get; set; }

    // Insurance information
    public string InsuranceNumberId { get; set; }
    public string InsuranceCompany { get; set; }
    public DateTime InsuranceExpiryDate { get; set; }

    // Doctor office information
    public string DoctorOfficeAddress { get; set; }
    public string DoctorOfficeCity { get; set; }
    public string DoctorOfficeZipCode { get; set; }
    public string DoctorOfficePhone { get; set; }

    // Medical information
    public string BloodType { get; set; }
    public string Allergies { get; set; }
    public string ChronicConditions { get; set; }
    public string Medications { get; set; }
    public string Notes { get; set; }
}