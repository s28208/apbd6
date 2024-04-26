using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial5.Models;
using Tutorial5.Models.DTOs;

namespace Tutorial5.Controllers;

[ApiController]
// [Route("api/animals")]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    public IActionResult GetAnimals(string orderBy= "")
    {
        // Otwieramy połączenie
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        // Definiujemy commanda
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM Animal";
        
        // Wykonanie commanda
        var reader = command.ExecuteReader();
        
        List<Animal> animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("idAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");
        int descriptionOrdinal = reader.GetOrdinal("Description");
        int categoryOrdinal = reader.GetOrdinal("Category");
        int areaOrdinal = reader.GetOrdinal("Area");

        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(nameOrdinal),
                Description = reader.GetString(descriptionOrdinal),
                Category = reader.GetString(categoryOrdinal),
                Area = reader.GetString(areaOrdinal)
            });
        }

        
        if (!string.IsNullOrEmpty(orderBy))
        {
            switch (orderBy.ToLower())
            {
                case "name":
                    animals = animals.OrderBy(a => a.Name).ToList();
                    break;
                case "description":
                    animals = animals.OrderBy(a => a.Description).ToList();
                    break;
                case "category":
                    animals = animals.OrderBy(a => a.Category).ToList();
                    break;
                case "area":
                    animals = animals.OrderBy(a => a.Area).ToList();
                    break;
                default:
                    return BadRequest("Invalid argument");
            }
        }
        else
        {
            animals = animals.OrderBy(a => a.Name).ToList();
        }
        
        
        
        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {
        // Otwieramy połączenie
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        // Definiujemy commanda
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES(@animalName, @animalDescription, @animalCategory, @animalArea)";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        command.Parameters.AddWithValue("@animalDescription", animal.Description);
        command.Parameters.AddWithValue("@animalCategory", animal.Category);
        command.Parameters.AddWithValue("@animalArea", animal.Area);

        command.ExecuteNonQuery();
        
        return Created("", null);
    }
    
    [HttpPut("{idAnimal}")]
    public IActionResult UpdateAnimal(int idAnimal, [FromBody] UpdateAnimal animal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText =
            "UPDATE Animal SET Name = @animalName, Description = @animalDescription, Category = @animalCategory, Area = @animalArea WHERE idAnimal = @animalId;";
        command.Parameters.Add("@animalId", SqlDbType.Int).Value = idAnimal;
        command.Parameters.Add("@animalName", SqlDbType.NVarChar).Value = animal.Name;
        command.Parameters.Add("@animalDescription", SqlDbType.NVarChar).Value = animal.Description;
        command.Parameters.Add("@animalCategory", SqlDbType.NVarChar).Value = animal.Category;
        command.Parameters.Add("@animalArea", SqlDbType.NVarChar).Value = animal.Area;
        int countRow = command.ExecuteNonQuery();
        if (countRow > 0)
        {
            return Ok();
        }
        else
        {
            return NotFound("Animal with specified ID not found.");
        }
    }

    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int idAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "DELETE FROM Animal WHERE idAnimal = @idA ";
        command.Parameters.AddWithValue("@idA", idAnimal);

        int countRow = command.ExecuteNonQuery();
        if (countRow > 0)
        {
            return Ok();
        }
        else
        {
            return NotFound("Animal with specified ID not found.");
        }
    }
    
    
}