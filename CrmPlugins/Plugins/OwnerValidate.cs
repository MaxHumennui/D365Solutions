using System;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;

namespace CrmPlugins
{
    public class OwnerValidate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Extract the tracing service for use in debugging sandboxed plug-ins.  
            // If you are not registering the plug-in in the sandbox, then you do  
            // not have to add any tracing service related code.  
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference which you will need for  
            // web service calls.  
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity rent = (Entity)context.InputParameters["Target"];

                try
                {
                    // Plug-in business logic goes here.

                    EntityReference customer = rent.GetAttributeValue<EntityReference>("cr59f_customer");

                    string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='cr59f_rent'>
                                        <attribute name='cr59f_customer' />
                                        <attribute name='statuscode' />
                                        <attribute name='cr59f_rentid' />
                                        <order attribute='cr59f_carclass' descending='false' />
                                        <filter type='and'>
                                          <condition attribute='cr59f_customer' operator='eq' uiname='" + customer.Name + @"' uitype='" + customer.LogicalName + @"' value='" + customer.Id + @"' />
                                        </filter>
                                      </entity>
                                    </fetch>";

                    EntityCollection result = service.RetrieveMultiple(new FetchExpression(fetch));

                    int i = 0;

                    foreach (var c in result.Entities)
                    {
                        OptionSetValue opt = (OptionSetValue)c.Attributes["statuscode"];
                        if (opt.Value == 257700002)
                            i++;
                    }

                    if (i >= 10)
                        throw new InvalidPluginExecutionException("Can create only 10 rents with status 'Renting' per one owner!");
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in MyPlug-in.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("MyPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
