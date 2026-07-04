using Core.Entities;
using Core.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.Patient;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        //private readonly IPatientRepository patientRepo;
        //public MedLinkDbContext DbContext { get; }

        //public PatientController(IPatientRepository patientRepository)
        //{
        //    patientRepo = patientRepository;
        //}



        //[HttpGet("Get-Patient-By-Id/{id}")]
        //public IActionResult GetById(string id)
        //{
        //    Patient patient = patientRepo.GetById(id);
        //    var user = DbContext.Users.FirstOrDefault(u => u.Id == id);
        //    if (patient != null)
        //    {
        //        PatientDto patientDto = new PatientDto
        //        {
        //            Name = user.Name,
        //            PhoneNumber = user.PhoneNumber,
        //            Email = user.Email,
        //            Address = patient.Address,
        //            Age = patient.Age,
        //            Gender = patient.Gender,
        //        };
        //        return Ok(patientDto);
        //    }
        //    return NotFound();
        //}

        //[HttpGet("Get-patient-by-name/{name}")]
        //public IActionResult GetByName(string name)
        //{
        //    Patient patient = patientRepo.GetByUsername(name);
        //    var user = DbContext.Users.FirstOrDefault(u => u.Name == name);

        //    if (patient != null)
        //    {
        //        PatientDto patientDto = new PatientDto
        //        {
        //            Id = patient.Id,
        //            Name = user.Name,
        //            PhoneNumber = user.PhoneNumber,
        //            Email = user.Email,
        //            Address = patient.Address,
        //            Age = patient.Age,
        //            Gender = patient.Gender,
        //        };
        //        return Ok(patientDto);
        //    }
        //    return NotFound();
        //}

        //[HttpPost("Add-Patient")]
        //public IActionResult Create(PatientDto patientDto)
        //{
        //    if (patientDto != null)
        //    {
        //        Patient patient = new Patient
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            Name = patientDto.Name,
        //            PhoneNumber = patientDto.PhoneNumber,
        //            Email = patientDto.Email,
        //            Address = patientDto.Address,
        //            Age = patientDto.Age,
        //            Gender = patientDto.Gender,
        //        };
        //        patientRepo.Add(patient);
        //        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
        //    }
        //    return BadRequest();
        //}

        //[HttpPut("Edit-Patient-info/{id}")]
        //public IActionResult Update(Patient NewPatient)
        //{
        //    var patient = patientRepo.GetById(NewPatient.Id);
        //    if (patient != null)
        //    {
        //        patient.Name = NewPatient.Name;
        //        patient.PhoneNumber = NewPatient.PhoneNumber;
        //        patient.Email = NewPatient.Email;
        //        patient.Address = NewPatient.Address;

        //        patientRepo.Update(patient);

        //        return NoContent();
        //    }
        //    return NotFound();
        //}
    }
}
