
# Foundry Analytics

API used for Client, Content, and User Events.

Examples of events that can be captured:

**Client Events**

    type: 'page-view'

**Content Events**

    type: 'launch'
    type: 'view'
    
**User Events**

    type: 'logged-in'

The Analytics API will accept any type of analytics event and can accept a data filter to retrieve events by type.

    filter: 'type=mycustomtype'

