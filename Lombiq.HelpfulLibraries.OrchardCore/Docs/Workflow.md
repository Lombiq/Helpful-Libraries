# Lombiq Helpful Libraries - Orchard Core Libraries - Workflows for Orchard Core

## Extensions

- `WorkflowManagerExtensions`: Adds `IWorkflowManager` extension methods for specific workflow events and `IEnumerable<IWorkflowManager>` extension methods for triggering workflow events only if there is a workflow manager.

## Activities

- `SimpleEventActivityBase`: A base class for a simple workflow event that only has a `Done` result.
- `SimpleEventActivityDisplayDriverBase`: A base class for a simple workflow event driver that only displays a title, description and optional icon in a conventional format.
