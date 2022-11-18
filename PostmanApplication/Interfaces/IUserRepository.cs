using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostmanApplication.Interfaces
{
    public interface IUserRepository
    {
        IUser Get(string userId);
    }
}
