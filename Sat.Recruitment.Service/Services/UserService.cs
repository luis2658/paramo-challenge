using Sat.Recruitment.Service.Abstract;
using Sat.Recruitment.Service.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Sat.Recruitment.Service.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new List<User>();
        public async Task<(bool, string)> RegisterUser(string name, string email, string address, string phone, string userType, string money)
        {
            var newUser = CreateUser(name, email, address, phone, userType, money);

            await ReadUsersFromFile();

            var (isSuccess, errors) = ValidateUser(newUser);

            return (isSuccess, errors);
        }

        private User CreateUser(string name, string email, string address, string phone, string userType, string money)
        {
            var newUser = new User
            {
                Name = name,
                Email = NormalizeEmail(email),
                Address = address,
                Phone = phone,
                UserType = userType,
                Money = ParseUserMoney(userType, money)
            };

            return newUser;
        }

        private (bool, string) ValidateUser(User newUser)
        {
            var isSuccess = false;
            var errors = "";

            try
            {
                var isDuplicated = false;
                foreach (var user in _users)
                {
                    if (user.Email == newUser.Email
                        ||
                        user.Phone == newUser.Phone)
                    {
                        isDuplicated = true;
                    }
                    else if (user.Name == newUser.Name)
                    {
                        if (user.Address == newUser.Address)
                        {
                            isDuplicated = true;
                            throw new Exception("User is duplicated");
                        }
                    }
                }

                if (!isDuplicated)
                {
                    Debug.WriteLine("User Created");
                    isSuccess = true;
                    errors = "User Created";

                }
                else
                {
                    Debug.WriteLine("The user is duplicated");
                    isSuccess = false;
                    errors = "The user is duplicated";
                }
            }
            catch
            {
                Debug.WriteLine("The user is duplicated");
                isSuccess = false;
                errors = "The user is duplicated";
            }

            return (isSuccess, errors);
        }

        private async Task ReadUsersFromFile()
        {
            var path = Directory.GetCurrentDirectory() + "/Files/Users.txt";

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            StreamReader reader = new StreamReader(fileStream);

            while (reader.Peek() >= 0)
            {
                var line = await reader.ReadLineAsync();
                var user = new User
                {
                    Name = line.Split(',')[0].ToString(),
                    Email = line.Split(',')[1].ToString(),
                    Phone = line.Split(',')[2].ToString(),
                    Address = line.Split(',')[3].ToString(),
                    UserType = line.Split(',')[4].ToString(),
                    Money = decimal.Parse(line.Split(',')[5].ToString()),
                };
                _users.Add(user);
            }
        }

        private decimal ParseUserMoney(string userType, string money)
        {
            decimal percentage = Convert.ToDecimal(1);
            decimal updatedMoney = decimal.Parse(money);

            switch (userType)
            {
                case "Normal":

                    if (decimal.Parse(money) > 100)
                    {
                        percentage = Convert.ToDecimal(0.12);
                        //If new user is normal and has more than USD100                    
                    }
                    if (decimal.Parse(money) < 100)
                    {
                        if (decimal.Parse(money) > 10)
                        {
                            percentage = Convert.ToDecimal(0.8);
                        }
                    }

                    break;

                case "SuperUser":

                    if (decimal.Parse(money) > 100)
                    {
                        percentage = Convert.ToDecimal(0.20);
                    }

                    break;

                case "Premium":

                    if (decimal.Parse(money) > 100)
                    {
                        //var gif = decimal.Parse(money) * 2;
                        percentage = Convert.ToDecimal(2);
                    }
                    break;
            }

            var gif = decimal.Parse(money) * percentage;
            return updatedMoney + gif;
        }

        private string NormalizeEmail(string Email)
        {
            var aux = Email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

            var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);

            aux[0] = atIndex < 0 ? aux[0].Replace(".", "") : aux[0].Replace(".", "").Remove(atIndex);

            return string.Join("@", new string[] { aux[0], aux[1] });
        }

        public string ValidateErrors(string name, string email, string address, string phone)
        {
            var errors = "";

            if (name == null)
            {
                //Validate if Name is null
                errors = "The name is required";
            }

            if (email == null)
            {
                //Validate if Email is null
                errors = errors + " The email is required";
            }

            if (address == null)
            {
                //Validate if Address is null
                errors = errors + " The address is required";
            }

            if (phone == null)
            {
                //Validate if Phone is null
                errors = errors + " The phone is required";
            }

            return errors;
        }
    }
}
