# RedisRepo Setup
Easy Redis Client for C sharp dot net

## Startup.cs
``` javascript
 public void ConfigureServices(IServiceCollection services)
        {
        // configure redis
         services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("127.0.0.1:6379,allowAdmin=true"));
        
        // DI
            services.AddTransient<EmployeeRespository, EmployeeRespository>();
            services.AddTransient<RedisContext<Employee>, RedisContext<Employee>>(); 
   
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConnectionMultiplexer connectionMultiplexer)
        {
            connectionMultiplexer.GetServer("127.0.0.1:6379").FlushAllDatabases();
        }
 ``` 
 ## Entity
  ``` javascript
 public class Employee: BaseEntity<int>
    {
        public string Name { get; set; }
        public int Salary { get; set; }

        public Employee()
        {
        }
        public Employee(int id, string name, int salary)
        {
            Id = id;
            Name = name;
            Salary = salary;
        }
        public Employee Update(string name, int salary)
        {
            Name = name;
            Salary = salary;
            return this;
        }
    }
  ``` 
 ## Cache Repository
 ``` javascript
using RedisRepo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Models
{
    public class EmployeeRespository
    {
        private List<Employee> _employees = new List<Employee>()
        {
            new Employee(1,"Kamal",10000),
            new Employee(2,"Ruwan",5000),
            new Employee(3,"Saman",15000),
            new Employee(4,"Chamara",45000),
            new Employee(5,"Nihal",85000)
        };
        private readonly RedisContext<Employee> _cache;

        public EmployeeRespository(RedisContext<Employee> cache)
        {
            _cache = cache.SetDatabase(nameof(Employee));
        }
        public async Task Add(Employee model)
        {
            var maxid = _employees.Max(p => p.Id);
            var modelx = new Employee(++maxid, model.Name, model.Salary);
            _employees.Add(modelx);
            await _cache.AddOrUpdate(modelx);
        }
        public async Task Update(Employee model)
        {
            var result = _employees.SingleOrDefault(p => p.Id == model.Id);
            if (result != null)
            {
                result.Update(model.Name, model.Salary);
                await _cache.AddOrUpdate(model);
            }
        }
        public async Task Delete(int id)
        {
            _employees.RemoveAll(p => p.Id == id);
            await _cache.Delete(id);
        }
        public async Task<List<Employee>> Read()
        {
            var result = await _cache.Get();
            if (!result.Item1)
            {
                var dbresult = _employees;
                return await _cache.RefillIfNot(dbresult);
            }
            else
            {
                return result.Item2;
            }
        }
        public async Task<Employee> Read(int id)
        {
            var result = await _cache.GetById(id);
            if (result.Item1)
            {
                return result.Item2;
            }
            else
            {
                return (await Read()).SingleOrDefault(p => p.Id == id);
            }
        }
    }
}
 ```
 ## Api Controller

 ``` 
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
 ```
