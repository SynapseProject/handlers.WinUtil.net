// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Synapse.Core;
using Synapse.Handlers.WinUtil;

namespace Synapse.Handler.WinUtil.UnitTests
{
    [TestFixture]
    public class ServiceHandlerTests
    {
        string _serviceName = "MyService";
        ServiceStartMode _startMode = ServiceStartMode.Manual;
        const string _pathName = @"""c:\siew hooi\synapse\controller\Synapse.Server.exe"" ""c:\siew hooi\synapse\controller\Synapse.Server.config.yaml""";
        const string _machineName = "localhost";
        

        [OneTimeSetUp]
        public void Init()
        {
        }
        [Test]
        public void CreateStartStopDeleteService()
        {
            _startMode = ServiceStartMode.Manual;

            // delete service if exists
            ServiceUtil.DeleteService( _serviceName, _machineName );

            CreateService();

            _startMode = ServiceStartMode.Automatic;
            ChangeStartMode();

            StartService();

            StopService();

            DeleteService();
        }
        public void ChangeStartMode()
        {
            ServiceReturnCode rtnCode = ServiceUtil.ChangeStartMode( _serviceName, _machineName, _startMode );
            ServiceConfig config = ServiceUtil.QueryService( _serviceName, _machineName );
            Assert.AreEqual( ServiceReturnCode.Success, rtnCode );
            Assert.AreEqual( _startMode, config.StartMode );
        }
        public void CreateService()
        {
            ServiceReturnCode rtnCode = ServiceUtil.CreateService( _serviceName, _machineName, $"{_serviceName} Display Name", $"{_serviceName} Description", _pathName, _startMode );

            // query service
            ServiceConfig config = ServiceUtil.QueryService( _serviceName, _machineName );
            Assert.AreEqual( ServiceReturnCode.Success, rtnCode );
            Assert.AreEqual( _serviceName, config.Name );
        }
        public void StartService()
        {
            bool started = ServiceUtil.Start( _serviceName, _machineName, 30000 );

            Assert.AreEqual( true, started );
        }
        public void DeleteService()
        {
            ServiceReturnCode rtnCode = ServiceUtil.DeleteService( _serviceName, _machineName );
            Assert.AreEqual( ServiceReturnCode.Success, rtnCode );
        }
        public void StopService()
        {
            bool stopped = ServiceUtil.Stop( _serviceName, _machineName, 30000 );
            Assert.AreEqual( true, stopped );
        }
    }
}
