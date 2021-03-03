using System.Threading;

namespace GameServer
{
	public interface IServer
	{
		void Run(IMessageHandler handler, CancellationToken cancellationToken);
	}
}