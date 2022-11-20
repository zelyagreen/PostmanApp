using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostmanApplication.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Метод получения карточки пользователя по его id
        /// </summary>
        IUser Get(string userId);
    }
}
