# D365 Update any Entity and Children
The solution offers two global actions, one to allow updating an entity without knowing its type a priori. The other allows updating the children of an entity whose type is not known a priori, starting with the name of the relationship.

The idea came from the need to perform updates on entity and/or children (link-entity) following a cloning operation done with the [D365 Configurable Entity Cloner](https://github.com/danielejdm/D365-ConfigurableEntityCloner) solution.
In these scenarios, the type of the entity and children is not known a priori. So to avoid writing dedicated components for each scenario, I tried to develop generic components to perform field updates, in a configurable and entity type-independent manner.

### Features

- Update attributes on entity of any type.
- Update attributes on link-entity (children) of any type.

## Getting started

### Prerequisites

Dynamics 365 v9.2+

### Install

Build the project and deploy the assembly into your environment.

### Usage

#### Update Generic Entity

Updates attributes on entity.
 ![image](https://user-images.githubusercontent.com/34159960/214939164-13990057-4fb6-4e06-a3bd-690495247bc4.png)

Integrate the action <i>Update Generic Entity</i> where you want to trigger it (Workflow, Javascript function, etc.).

##### Parameters-Configuration

- RecordInfo:
  - "RecordId" as string
    - example <i>D06F15DB-EEA5-4086-996B-47987DDB7628</i>
  - "RecordUrl" (string)
    - example: <i>https<nolink>://myorg.crm.dynamics.com/main.aspx?etn=account&pagetype=entityrecord&id=%7B91330924-802A-4B0D-A900-34FD9D790829%7D</i>

- EntityName: schema-name of the entity
  - required only when <i>RecordInfo</i> is a (string)RecordId
  - example: <i>account</i>

- UpdateInactive
  - false -> the entity is updated only if it is in an active state
  - true -> the entity is updated whatever its state is (active/inactive)

- AttributesNameValue: a string containing a sequence of <i>Attribute-Name/Attribute-Value</i>
  - format: <i>jdm_field1=>val1<>jdm_field2=>val2<>jdm_field3=>val3...</i>
    - <> is a separator for the different <i>Attribute-Name/Attribute-Value</i> pair
    - the value for the attributes is a normal string: it is parsed by the module to the proper <i>attribute-type</i>
    - to set a null value for an attribute, simply provide and "empty value":
      - example: <i>jdm_field1=><>jdm_field2=><>jdm_field3=>val3</i>

#### Update Entity Children

Updates attributes on link-entities.
 ![image](https://user-images.githubusercontent.com/34159960/214940339-942ec2c1-817a-4b15-9b76-fbf3b83116e9.png)

Integrate the action <i>Update Entity Children</i> where you want to trigger it (Workflow, Javascript function, etc.).

##### Parameters-Configuration

- RecordInfo:
  - "RecordId" as string
    - example: <i>D06F15DB-EEA5-4086-996B-47987DDB7628</i>
  - "RecordUrl" (string)
    - example: <i>https<nolink>://myorg.crm.dynamics.com/main.aspx?etn=account&pagetype=entityrecord&id=%7B91330924-802A-4B0D-A900-34FD9D790829%7D</i>

- RelationshipName: schema-name of the relationship
  - required
  - example: <i>contact_customer_accounts</i>

- UpdateInactive
  - false -> the link-entity is updated only if it is in an active state
  - true -> the link-entity is updated whatever its state is (active/inactive)

- AttributesNameValue: a string containing a sequence of <i>Attribute-Name/Attribute-Value</i>
  - format: jdm_field1=>val1<>jdm_field2=>val2<>jdm_field3=>val3...
    - <> is a separator for the different <i>Attribute-Name/Attribute-Value</i> pair.
    - the value for the attributes is a normal string: it is parsed by the module to the proper <i>attribute-type</i>
    - to set a null value for an attribute, simply provide and "empty value":
      - example: <i>jdm_field1=><>jdm_field2=><>jdm_field3=>val3</i>

## Back matter

### Acknowledgements

Thanks to all who helped inspire this solution.

### License

This project is licensed under the [GPLv3](https://www.gnu.org/licenses/gpl-3.0.html).
