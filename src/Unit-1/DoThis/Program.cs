using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            // initialize MyActorSystem
            MyActorSystem = ActorSystem.Create("MyActorSystem");

            // Three types of Props creation syntax:
            // typeof syntax - not recommended due to type safety reasons, so commented out and replaced by generic syntax
            // lambda syntax - good choice
            // generic syntax - good choice too

            // consoleWriterProps = Props.Create(typeof(ConsoleWriterActor));
            Props consoleWriterProps = Props.Create<ConsoleWriterActor>();
            IActorRef consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "consoleWriterActor");

            // TODO: find out why one argument constructor is using without implementing in FileValidatorActor.cs
            //Props validationActorProps = Props.Create(() => new FileValidatorActor(consoleWriterActor));
            //IActorRef validationActor = MyActorSystem.ActorOf(validationActorProps, "validationActor");

            
            Props tailCoordinatorProps = Props.Create<TailCoordinatorActor>();
            IActorRef tailCoordinatorActor = MyActorSystem.ActorOf(tailCoordinatorProps, "tailCoordinatorActor");

            //pass tailCoordinatorActor to fileValidatorActorProps
            Props fileValidatorActorProps = Props.Create(() => new FileValidatorActor(consoleWriterActor));
            IActorRef validationActor = MyActorSystem.ActorOf(fileValidatorActorProps, "validationActor");

            Props consoleReaderProps = Props.Create<ConsoleReaderActor>();
            IActorRef consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps, "consoleReaderActor");

            // tell console reader to begin
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.WhenTerminated.Wait();

        }




    }
    #endregion
}
