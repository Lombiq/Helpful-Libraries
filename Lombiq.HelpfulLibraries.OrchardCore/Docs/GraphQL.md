# Lombiq Helpful Libraries - Orchard Core Libraries - GraphQL for Orchard Core

Contains supplementary types for using GraphQL in Orchard Core.

## Extensions

- `TotalOfContentTypeBuilder`: An `IContentTypeBuilder` which adds the `totalOfContentType` integer field to your top level `ContentItem`s in your schema. Needs to be registered as `Scoped`.

## Services

- `PartIndexAliasProvider`: Eliminates boilerplate for `IIndexAliasProvider` for indexes with a name ending in `PartIndex`.
