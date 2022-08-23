using System.Threading.Tasks;

namespace Sat.Recruitment.Service.Abstract
{
    public interface IUserService
    {
        Task<(bool, string)> RegisterUser(string name, string email, string address, string phone, string userType, string money);
        string ValidateErrors(string name, string email, string address, string phone);
    }
}