using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PostmanApplication.Interfaces;

namespace PostmanApplication
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // гипотетический лист пользователей, которых мы вытянули из гипотетической базы
            var usersList = new List<User>();
            usersList.Add(new User { Address = "testAd", DeliveryMethod = 1, Id = "testId" });
            usersList.Add(new User { Address = "testAd1", DeliveryMethod = 2, Id = "testId1" });
            usersList.Add(new User { Address = "testAd2", DeliveryMethod = 3, Id = "testId2" });
            usersList.Add(new User { Address = "testAd3", DeliveryMethod = 1, Id = "testId3" });
            // гипотетический лист сообщений
            var messages = new List<Message>();
            messages.Add(new Message { MessageText = "testText", UserId = "testId" });
            messages.Add(new Message { MessageText = "testText1", UserId = "testId1" });
            messages.Add(new Message { MessageText = "testText2", UserId = "testId2" });
            messages.Add(new Message { MessageText = "testText3", UserId = "testId3" });
            var postman = new Postmans(messages, usersList);
            Console.ReadKey();
        }

        public class Postmans : ISender
        {
            private readonly int _maxThreads = 10;
            /// <summary>
            /// Список, куда следует поместить все сообщения, которые не удалось доставить
            /// </summary>
            private IEnumerable<IMessage> _failedMessages;
            private readonly Mutex m = new Mutex();
            private readonly string filePathForMessage = @"C:\Users\userg\source\repos\GetAllSequences\PostmanApplicationlication\bin\Debug\Messages";
            private UserRepository _userRepository;
            private static TimerCallback _callBack { get; set; }
            private static System.Threading.Timer _timer { get; set; }


            public Postmans(IEnumerable<Message> messages, List<User> users)
            {
                _userRepository = new UserRepository(users);
                var newThread = new Thread(async () =>
                {
                    await RunTasks(messages, _userRepository);
                });
                newThread.Start();
                _callBack = new TimerCallback(SaveFailMessage);
                _failedMessages = new List<IMessage>();
                _timer = new System.Threading.Timer(_callBack, null, 0, 60 * 1000);
            }

            private async void SaveFailMessage(object state)
            {
                if (_failedMessages.Count() > 0)
                {
                    foreach (var failMessage in _failedMessages)
                        using (FileStream stream = new FileStream(Path.Combine(filePathForMessage, $@"\Fails\ErrorMessages{DateTime.Now.ToString("dd/MM/yyyy/HH:mm::ss")}.txt"), FileMode.Create))
                        using (StreamWriter sw = new StreamWriter(stream))
                        {
                            await sw.WriteLineAsync($"FailMessage: \n {failMessage.MessageText} \n UserId: \n {failMessage.UserId} ");
                        }
                }
            }

            public async Task RunTasks(IEnumerable<Message> messages, UserRepository userRepository)
            {
                var repository = userRepository;
                var tasks = new List<Task>();

                foreach (var message in messages)
                {
                    var userInfo = repository.Get(message.UserId);

                    switch (userInfo.DeliveryMethod)
                    {
                        case (1):
                            tasks.Add(new Task(async () => await SendTelegram(message.MessageText, userInfo.Address, userInfo.Id)));
                            break;

                        case (2):
                            tasks.Add(new Task(async () => await SendPhone(message.MessageText, userInfo.Address, userInfo.Id)));

                            break;

                        case (3):
                            tasks.Add(new Task(async () => await SendEmail(message.MessageText, userInfo.Address, userInfo.Id)));

                            break;
                    }
                }

                Parallel.ForEach(tasks, new ParallelOptions { MaxDegreeOfParallelism = _maxThreads }, task =>
                {
                    task.Start();
                });

                //tasks.Add(Task.Run(() => Send(messages)));
                //// ...
                await Task.WhenAll(tasks);
            }

            /// <summary>
            /// Отправляет сообщения из списка messages пользователям
            /// Сообщение отправляется методом, указанным в записи пользователя
            /// по адресу, указанным в записи пользователя
            /// В случае, если сообщение не удалось доставить, помещает его в  FailedMessages
            /// </summary>
            /// <param name="messages"> сообщение для клиента </param>
            /// <param name="address"> адрес клиента </param>
            public async Task SendTelegram(string message, string address, string userId)
            {
                m.WaitOne();
                try
                {
                    DateTime dateTime = DateTime.Now;
                    long unixTime = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();

                    Console.WriteLine($"{Thread.CurrentThread.GetHashCode()}");
                    using (FileStream stream = new FileStream($@"C:\Messages\{userId} {unixTime}.txt", FileMode.Create))
                    using (StreamWriter sw = new StreamWriter(stream))
                    {
                        await sw.WriteLineAsync($"Message: \n {message} \nUserId: \n {userId} ");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    var failedMessage = new Message { MessageText = message, UserId = userId };
                    _failedMessages.ToList().Add(failedMessage);
                }
                finally
                {
                    m.ReleaseMutex();
                }
            }



            /// <summary>
            /// Отправляет сообщения из списка messages пользователям
            /// Сообщение отправляется методом, указанным в записи пользователя
            /// по адресу, указанным в записи пользователя
            /// В случае, если сообщение не удалось доставить, помещает его в  FailedMessages
            /// </summary>
            /// <param name="messages"> сообщение для клиента </param>
            /// <param name="address"> адрес клиента </param>
            public async Task SendPhone(string message, string address, string userId)
            {
                m.WaitOne();
                try
                {
                    DateTime dateTime = DateTime.Now;
                    long unixTime = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
                    Console.WriteLine($"{Thread.CurrentThread.GetHashCode()}");
                    using (FileStream stream = new FileStream($@"C:\Messages\{userId} {unixTime}.txt", FileMode.Create))
                    using (StreamWriter sw = new StreamWriter(stream))
                    {
                        await sw.WriteLineAsync($"Message: \n {message} \nUserId: \n {userId} ");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    var failedMessage = new Message { MessageText = message, UserId = userId };
                    _failedMessages.ToList().Add(failedMessage);
                }
                finally
                {
                    m.ReleaseMutex();
                }
            }

            /// <summary>
            /// Отправляет сообщения из списка messages пользователям
            /// Сообщение отправляется методом, указанным в записи пользователя
            /// по адресу, указанным в записи пользователя
            /// В случае, если сообщение не удалось доставить, помещает его в  FailedMessages
            /// </summary>
            /// <param name="messages"> сообщение для клиента </param>
            /// <param name="address"> адрес клиента </param>
            public async Task SendEmail(string message, string address, string userId)
            {
                m.WaitOne();
                try
                {
                    DateTime dateTime = DateTime.Now;
                    long unixTime = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
                    Console.WriteLine($"{Thread.CurrentThread.GetHashCode()}");
                    using (FileStream stream = new FileStream($@"C:\Messages\{userId} {unixTime}.txt", FileMode.Create))
                    using (StreamWriter sw = new StreamWriter(stream))
                    {
                        await sw.WriteLineAsync($"Message: \n {message} \nUserId: \n {userId} ");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    var failedMessage = new Message { MessageText = message, UserId = userId };
                    _failedMessages.ToList().Add(failedMessage);
                }
                finally
                {
                    m.ReleaseMutex();
                }
            }


        }

        public class User : IUser
        {
            public string Id { get; set; }

            public int DeliveryMethod { get; set; }

            public string Address { get; set; }

        }

        public class UserRepository : IUserRepository
        {
            private List<User> _users { get; set; }

            public UserRepository(List<User> users)
            {
                _users = users;
            }

            public IUser Get(string userId)
            {
                var user = _users.FirstOrDefault(us => us.Id == userId);
                return user;
            }
        }

        public class Message : IMessage
        {
            public string UserId { get; set; }

            public string MessageText { get; set; }
        }
    }
}
