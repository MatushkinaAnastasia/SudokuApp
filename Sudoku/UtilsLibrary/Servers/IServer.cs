using System.Threading;

namespace UtilsLibrary.Servers
{
	public interface IServer
	{
		void Run(IMessageHandler handler, CancellationToken cancellationToken);
	}
}
