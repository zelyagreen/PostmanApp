using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostmanApplication.Interfaces
{
    public interface IUser
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Метод доставки сообщения, напаример: 
        /// 0 -не нужна доставка, 1 - Телеграмм, 2 - СМС, 3 - e-mail, 4 - WhatsApp и тд и тп
        /// </summary>
        int DeliveryMethod { get; }
        /// <summary>
        /// Адресс, по которму будут отправляться сообщания. Зависит от метода доставки:
        /// аккаунт Телеграмм, номер телефона, e-mail и тд
        /// </summary>
        string Address { get; }
    }
}
