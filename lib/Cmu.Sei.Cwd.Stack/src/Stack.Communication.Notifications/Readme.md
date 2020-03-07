# Stack.Communication.Notifications

Package for handling notifications to the communication api

> Dependencies

[Stack.DomainEvents](https://code.sei.cmu.edu/bitbucket/projects/CWD/repos/stack.domainevents/browse)

> Startup.cs config

    // notification handler creates notifications
    services.AddSingleton<IDomainEventHandler, NotificationHandler>();

    // handle domain events with the DomainEventDelegator directly
    services.AddSingleton<IDomainEventDelegator, DomainEventDelegator>();

    // domain event dispatcher handles api events
    services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

> Define a notification strategy

    public class GroupAddNotificationStrategy : NotificationCreateStrategy
    {
        public GroupAddNotificationStrategy(CommunicationOptions communicationOptions, IDomainEvent domainEvent)
            : base(communicationOptions, domainEvent) { }

        public override string Label => "add";

        public override string Type => "groupadd";

        public string ToLocalUrl(string prefix, string id, string slug)
        {
            return string.Format("{0}/{1}/{2}/{3}", CommunicationOptions.ClientUrl, prefix, id, slug);
        }

        public async override Task<NotificationCreate> GetModel()
        {
            var group = DomainEvent.Entity as Group;

            var owners = group.Members.Select(m => m.AccountId).ToArray();

            var notification = new NotificationCreate
            {
                GlobalId = DomainEvent.Id,
                Subject = "Group Added",
                Body = string.Format("Group '{0}' was added.", DomainEvent.Name),
                Values = ToNotificationCreateValues(),
                Url = ToLocalUrl("group", group.Id, group.Slug),
                Recipients = owners
            };

            return notification;
        }
    }

> Example synchronous and asynchronous usage in a service class

    public IDomainEventDispatcher DomainEventDispatcher { get; }

    public DispatchService(
        IDomainEventDispatcher domainEventDispatcher,
        IIdentityResolver identityResolver,
        TRepository repository,
        IMapper mapper,
        IValidationHandler validationHandler)
            : base(identityResolver, repository, mapper, validationHandler)
    {
        DomainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
    }

    protected void Dispatch(DomainEvent @event)
    {
        var _ = Task.Run(() => DomainEventDispatcher.Dispatch(@event));
    }

    protected async Task DispatchAsync(DomainEvent @event)
    {
        await DomainEventDispatcher.DispatchAsync(@event);
    }

> Creating a domain event and calling the dispatcher

*note that the notification strategy `Type` should match the domain event `Type` and be unique. In this example it is `"groupadd"`. This directs the `NotificationStrategyFactory` on how to process the domain event.*

    public async Task<GroupDetail> Add(GroupCreate model)
    {
        var group = new Group
        {
            Description = model.Description,
            LogoUrl = model.LogoUrl,
            Name = model.Name,
            Summary = model.Summary,
            ParentId = string.IsNullOrWhiteSpace(model.ParentId) ? null : model.ParentId
        };

        var saved = await Repository.Add(group);

        Dispatch(new DomainEvent(saved, saved.Id, saved.Name, "groupadd"));

        return await GetById(saved.Id);
    }