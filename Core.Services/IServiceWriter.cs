using Core.Applications.Writers;
using Core.Internet.Smtp;

namespace Core.Services;

public interface IServiceWriter : IWriter
{
   void SendEmail(string message);

   void SendEmail(Address address, string message);
}