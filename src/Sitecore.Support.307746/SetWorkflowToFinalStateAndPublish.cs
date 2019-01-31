using System.Linq;
using Sitecore.ContentTesting.Helpers;
using Sitecore.ContentTesting.Pipelines.RevertContent;
using Sitecore.Data.Items;
using Sitecore.Publishing;

namespace Sitecore.Support
{
  public class SetWorkflowToFinalStateAndPublish : RevertContentProcessor
  {
    public override void Process(RevertContentArgs args)
    {
      var hostItemUri = args.TestDefinition.ParseContentItem();
      var hostItem =
        args.TestDefinition.Database.GetItem(hostItemUri.ItemID, hostItemUri.Language, hostItemUri.Version);
      if (hostItem != null)
      {
        var state = hostItem.State.GetWorkflowState();
        using (new EditContext(args.HostItem))
        {
          if (state != null)
          {
            args.HostItem.Fields[FieldIDs.WorkflowState].SetValue(state.StateID, false);
          }
          else
          {
            args.HostItem.Fields[FieldIDs.WorkflowState].SetValue(string.Empty, false);
          }
        }
      }

      var targets = PublishingHelper.GetTargets(args.HostItem);
      PublishManager.PublishItem(args.HostItem, targets.ToArray(), args.HostItem.Languages, false, false);
    }
  }
}