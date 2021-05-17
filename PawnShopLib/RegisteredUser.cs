using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PawnShopLib
{
    [Serializable]
    public abstract class RegisteredUser
    {
        protected string _hashPassword;
        /// <summary>
        /// Password property (only setter)
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when value contains whitespaces or smaller than 4 characters</exception>
        /// <exception cref="ArgumentNullException">Thrown when value is null</exception>
        public virtual string Password
        {
            set
            {
                if (value != null)
                {
                    const int minSize = 4;
                    if (value.Length >= minSize)
                    {
                        foreach (char symbol in value)
                        {
                            if (char.IsWhiteSpace(symbol))
                                throw new ArgumentException("Password should not contain whitespaces");
                        }

                        _hashPassword = GetHash(value);
                    }
                    else
                    {
                        throw new ArgumentException($"Password should have at least {minSize} characters");
                    }
                }
                else
                {
                    throw new ArgumentNullException("Password can`t be null", nameof(value));
                }
            }
        }
        public RegisteredUser(string password)
        {
            Password = password;
        }
        public virtual bool CheckPassword(string toCheck)
        {
            if (toCheck != null)
                return _hashPassword == GetHash(toCheck);
            else
                throw new ArgumentNullException(nameof(toCheck), "Possible password can`t be null");
        }
        protected virtual string GetHash(string toHash)
        {
            SHA512 sha512 = SHA512.Create();
            byte[] hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(toHash));
            return Convert.ToBase64String(hash);
        }
    }
}
