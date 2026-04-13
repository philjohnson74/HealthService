namespace HealthService.Patient.Api.Data;

public class InMemoryPatientRepository : IPatientRepository
{
    private static readonly List<Patient> _patients =
    [
        new() { Id = 1, NHSNumber = "485 777 3456", Name = "Alice Hartley",   DateOfBirth = new DateTime(1985, 3, 12), GPPractice = "Northside Medical Centre" },
        new() { Id = 2, NHSNumber = "321 654 9870", Name = "Ben Okafor",      DateOfBirth = new DateTime(1972, 7, 28), GPPractice = "Riverside Health Group" },
        new() { Id = 3, NHSNumber = "654 321 1234", Name = "Clara Mendez",    DateOfBirth = new DateTime(1990, 11, 5), GPPractice = "Southgate Surgery" },
        new() { Id = 4, NHSNumber = "789 012 3456", Name = "David Nguyen",    DateOfBirth = new DateTime(1964, 1, 19), GPPractice = "Northside Medical Centre" },
        new() { Id = 5, NHSNumber = "147 258 3690", Name = "Eleanor Walsh",   DateOfBirth = new DateTime(2001, 9, 30), GPPractice = "Eastfield Practice" },
    ];

    public Patient? GetById(int id) =>
        _patients.FirstOrDefault(p => p.Id == id);
}
