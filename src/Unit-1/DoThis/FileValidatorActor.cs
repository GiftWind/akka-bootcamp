using Akka.Actor;
using System;
using System.IO;

namespace WinTail
{
    class FileValidatorActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;
        
        
        public FileValidatorActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (String.IsNullOrEmpty(msg))
            {
                // signal that the user must provide correct input
                _consoleWriterActor.Tell(new Messages.NullInputError("Empty input. Please try again. \n"));

                // tell sender to continue doing its work
                Sender.Tell(new Messages.ContinueProcessing());
            }

            else
            {
                var valid = IsFileUri(msg);

                if (valid)
                {
                    // signal that input was coorrect (file exists)
                    _consoleWriterActor.Tell(new Messages.InputSuccess($"Starting processing for {msg}"));

                    // start coordinator
                    Context.ActorSelection("akka://MyActorSystem/user/tailCoordinatorActor").Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));
                }

                else
                {
                    //  signal that input is not the correct filepath
                    _consoleWriterActor.Tell(new Messages.ValidationError($"{msg} is not an existing URI"));

                    //tell sender to continue doing its work
                    Sender.Tell(new Messages.ContinueProcessing());
                }

            }
        }

        /// <summary>
        /// Checks if file exists at path provided by user
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsFileUri(string path)
        {
            return File.Exists(path);
        }
    }
}