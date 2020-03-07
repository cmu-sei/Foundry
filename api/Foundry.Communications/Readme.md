
# Foundry Communications

API for managing notifications for Foundry applications

**Create Notification**

    {
        subject: 'Notification Text',
        body: 'Text',
        url: 'http://...',
        values: [
            { key: 'label', value: 'important' },
            { key: 'icon', value: 'fa-arrow' }
        ],
        globalId: 'guid',
        priority: 0, //normal
        recipients: ['guid','guid']
    }
