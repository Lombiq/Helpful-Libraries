# Lombiq Helpful Libraries - Orchard Core Libraries - Workflows for Orchard Core

## Extensions

- `WorkflowManagerExtensions`: Adds `IWorkflowManager` extension methods for specific workflow events and `IEnumerable<IWorkflowManager>` extension methods for triggering workflow events only if there is a workflow manager.

## Activities

- `SimpleEventActivity`:  A base class for a simple workflow event that only has a `Done` result.
