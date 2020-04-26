namespace Redis.Models
{
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
}
