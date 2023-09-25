namespace Api.Dtos.Employee;

public class GetPaycheckDto
{
    public decimal BaseSalary { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary { get; set; }
}
