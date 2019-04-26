using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Synapse.Handlers.WinUtil;

namespace Synapse.Handler.WinUtil.UnitTests
{
    [TestFixture]
    public class ScheduledTaskHandlerTests
    {
        string _taskName = "tsh"; //"MyTask";
        const string _machineName = "localhost";

        [OneTimeSetUp]
        public void Init()
        {
        }
        [Test]
        public void EnableDisableScheduledTask()
        {

            QueryTask();

            EnableTask();

            DisableTask();

        }
        public void QueryTask()
        {
            ScheduledTaskConfig config = ScheduledTaskUtil.QueryStatus( _taskName, _machineName );
            Assert.AreNotEqual( "Unknown", config.State );
        }
        public void EnableTask()
        {
            bool succcess = ScheduledTaskUtil.Enable( _taskName, _machineName );
            Assert.AreEqual( true, succcess );
        }
        public void DisableTask()
        {
            bool succcess = ScheduledTaskUtil.Disable( _taskName, _machineName );
            Assert.AreEqual( true, succcess );
        }
    }
}
