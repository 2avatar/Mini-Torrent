using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Peer2Peer
{
    //Contruct
    [ServiceContract(CallbackContract = typeof(IPing1))]
    public interface IPing1
    {     
        [OperationContract(IsOneWay = true)]
        void BroadcastAddSocket(UserDataBinding user);
        [OperationContract(IsOneWay = true)]
        void BroadcastNewUser(UserDataBinding user);
        [OperationContract(IsOneWay = true)]
        void BroadcastRemoveSocket(UserDataBinding user);
        [OperationContract(IsOneWay = true)]
        void BroadcastPeerToConnect(string myPeer, string otherPeer);
    }

    // Implementatiom
    public class PingImplementation : IPing1
    {
        
        private List<UserDataBinding> allHosts = new List<UserDataBinding>();
        public UserDataBinding MyHost { get; set; }
        public IPing1 myChannel { private get; set; }
      
        public void BroadcastAddSocket(UserDataBinding user)
        {
            if (!allHosts.Exists(x => x.Username == user.Username))
                allHosts.Add(user);

                myChannel.BroadcastNewUser(MyHost);
        }

        public void BroadcastNewUser(UserDataBinding user)
        {
            if (!allHosts.Exists(x => x.Username == user.Username))
                allHosts.Add(user);

            Console.WriteLine(allHosts.Count);
            foreach (UserDataBinding u in allHosts)
            {
                Console.WriteLine(u.Username);
            }
        }

        public void BroadcastRemoveSocket(UserDataBinding user)
        {
            allHosts.Remove(user);
        }

        public void BroadcastPeerToConnect(string myPeer, string otherPeer)
        {
            if (MyHost.Username == otherPeer)
            {
                Console.WriteLine("my peer: " + myPeer + " other peer: " + otherPeer);              
                connectClient(allHosts.Find(x => x.Username.Equals(myPeer)));
            }
        }

        private void connectClient(UserDataBinding user)
        {
            Client.StartClient(MyHost.IP, user.PORT);         
        }
    }
    //Open Peer
    public class Peer
    {
        public UserDataBinding User { get; private set; }

        public IPing1 Channel;
        public PingImplementation Host;

    
        public Peer(UserDataBinding user)
        {
            User = user;
        }

        public void StartService()
        {
            var binding = new NetPeerTcpBinding();
            binding.Security.Mode = SecurityMode.None;

            var endpoint = new ServiceEndpoint(
                ContractDescription.GetContract(typeof(IPing1)),
                binding,
                new EndpointAddress("net.p2p://SimpleP2P"));

            Host = new PingImplementation();
            Host.MyHost = User;
           

            _factory = new System.ServiceModel.DuplexChannelFactory<IPing1>(new InstanceContext(Host), endpoint);

            var channel = _factory.CreateChannel();

            ((ICommunicationObject)channel).Open();

            // wait until after the channel is open to allow access.
            Channel = channel;
            Host.myChannel = Channel;
        }
        private DuplexChannelFactory<IPing1> _factory;

        public void StopService()
        {
            ((ICommunicationObject)Channel).Close();
            if (_factory != null)
                _factory.Close();
        }
        private readonly AutoResetEvent _stopFlag = new AutoResetEvent(false);
        public void Run()
        {
            Console.WriteLine("[ Starting Service ]");
            StartService();

            Console.WriteLine("[ Service Started ]");
            _stopFlag.WaitOne();

            Console.WriteLine("[ Stopping Service ]");
            StopService();

            Console.WriteLine("[ Service Stopped ]");
        }

        public void Stop()
        {
            _stopFlag.Set();
        }
    }
}

