using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostmanApplication.Interfaces
{
    public interface IMessage
    {
        /// <summary>
        /// Идентификатор пользователя, которому надо доставить сообщение
        /// </summary>
        string UserId { get; }
        /// <summary>
        /// Текст сообщения
        /// </summary>
        string MessageText { get; }
    }
}
