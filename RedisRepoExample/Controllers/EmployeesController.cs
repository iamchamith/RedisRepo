using Microsoft.AspNetCore.Mvc;
using Redis.Models;
using System.Threading.Tasks;

namespace Redis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeRespository _employeeRespository;
        public EmployeesController(EmployeeRespository employeeRespository)
        {
            _employeeRespository = employeeRespository;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        { 
            return Ok(await _employeeRespository.Read());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _employeeRespository.Read(id));
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee value)
        {
            await _employeeRespository.Add(value);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Employee value)
        {
            await _employeeRespository.Update(value);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _employeeRespository.Delete(id);
            return Ok();
        }
    }
}
