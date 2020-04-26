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
        private readonly CacheContext<Employee> _cache;

        public EmployeeRespository(CacheContext<Employee> cache)
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
