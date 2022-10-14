using Core.Services.Loggers;

namespace Core.Services;

public interface IAddServiceMessages
{
   void AddServiceMessages(params IServiceMessage[] serviceMessages);
}