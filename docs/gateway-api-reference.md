# Sample: Recommendations
- Install `Digital catalog experience API` see above.
- Install [Recommendations Gateway API module](https://github.com/VirtoSolutions/vc-module-experience-api/tree/master/samples/RecommendationsGatewayModule)
- Add to the platform configuration file `appsettings.json` the following settings from this example https://github.com/VirtoSolutions/vc-module-experience-api/blob/master/samples/RecommendationsGatewayModule/RecommendationsGatewayModule.Core/Configuration/samples/vc-recommendations.json
this settings add vc product association APIs as downstream for  product recommendations.
- Open GraphQL UI playground in the browser `http://{platform url}/ui/playground` to start consume API.

The recommendations example GraphQL request:

```json
  recommendations(scenario:"vc-recommendations" after: "10" first: 5 itemId: "8b7b07c165924a879392f4f51a6f7ce0")
  {
    items
    {
      scenario
      product
      {
        id
        name
      }
    }
  }
```
