using Akka.Actor;
using System;

namespace WinTail
{
    public class TailCoordinatorActor : UntypedActor
    {

        #region Message types

        /// <summary>
        /// Start tailing the file at user specified path
        /// </summary>
        public class StartTail
        {
            public string FilePath { get; private set; }
            public IActorRef ReporterActor { get; private set; }

            public StartTail(string filePath, IActorRef reporterActor)
            {
                FilePath = filePath;
                ReporterActor = reporterActor;
            }
        }

        /// <summary>
        /// Stop tailing file at user specified path
        /// </summary>
        public class StopTail
        {
            public StopTail(string fileName)
            {
                FileName = fileName;
            }

            public string FileName { get; private set; }
        }

        #endregion

        protected override void OnReceive(object message)
        {
            if (message is StartTail)
            {
                var msg = message as StartTail;
                 
                //Creating the child actor
                Context.ActorOf(Props.Create(() => new TailActor(msg.ReporterActor, msg.FilePath)));
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10, // maxNumberOfRetries
                TimeSpan.FromSeconds(30), //withinTimeRange
                x => //localOnlyDecider
                {
                    if (x is ArithmeticException) return Directive.Resume;
                    if (x is NotSupportedException) return Directive.Stop;
                    else return Directive.Restart;
                  
                });
        }
    }
}