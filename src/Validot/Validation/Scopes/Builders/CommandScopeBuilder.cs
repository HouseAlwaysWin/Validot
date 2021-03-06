namespace Validot.Validation.Scopes.Builders
{
    using System;

    using Validot.Specification.Commands;

    internal class CommandScopeBuilder<T> : ICommandScopeBuilder
    {
        private readonly ICommand _command;

        private readonly Func<ICommand, IScopeBuilderContext, ICommandScope<T>> _coreBuilder;

        private ErrorBuilder _errorsBuilder;

        private string _path;

        private Predicate<T> _executionCondition;

        public CommandScopeBuilder(ICommand command, Func<ICommand, IScopeBuilderContext, ICommandScope<T>> coreBuilder)
        {
            ThrowHelper.NullArgument(command, nameof(command));

            ThrowHelper.NullArgument(coreBuilder, nameof(coreBuilder));

            _command = command;
            _coreBuilder = coreBuilder;
        }

        public ICommandScope Build(IScopeBuilderContext context)
        {
            ThrowHelper.NullArgument(context, nameof(context));

            var core = _coreBuilder(_command, context);

            ThrowHelper.NullArgument(core, nameof(core));

            if (_errorsBuilder != null)
            {
                var error = _errorsBuilder.Build();

                core.ErrorMode = _errorsBuilder.Mode;

                if (!_errorsBuilder.IsEmpty)
                {
                    core.ErrorId = context.RegisterError(error);
                }
                else if (_errorsBuilder.Mode == ErrorMode.Override)
                {
                    core.ErrorId = context.DefaultErrorId;
                }
            }

            if (_path != null)
            {
                core.Path = _path;
            }

            if (_executionCondition != null)
            {
                core.ExecutionCondition = _executionCondition;
            }

            return core;
        }

        public bool TryAdd(ICommand command)
        {
            ThrowHelper.NullArgument(command, nameof(command));

            if (command is WithConditionCommand<T> withConditionCommand)
            {
                _executionCondition = withConditionCommand.ExecutionCondition;

                return true;
            }

            if (command is WithPathCommand withPathCommand)
            {
                _path = withPathCommand.Path;

                return true;
            }

            if (_errorsBuilder == null)
            {
                _errorsBuilder = new ErrorBuilder();
            }

            return _errorsBuilder.TryAdd(command);
        }
    }
}
