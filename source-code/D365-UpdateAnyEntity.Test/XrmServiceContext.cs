namespace UpdateAnyEntity.Test
{

	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.82")]
	public partial class XrmServiceContext : Microsoft.Xrm.Sdk.Client.OrganizationServiceContext
	{
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public XrmServiceContext(Microsoft.Xrm.Sdk.IOrganizationService service) : 
				base(service)
		{
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="UpdateAnyEntity.Test.Account"/> entities.
		/// </summary>
		public System.Linq.IQueryable<UpdateAnyEntity.Test.Account> AccountSet
		{
			get
			{
				return this.CreateQuery<UpdateAnyEntity.Test.Account>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="UpdateAnyEntity.Test.Contact"/> entities.
		/// </summary>
		public System.Linq.IQueryable<UpdateAnyEntity.Test.Contact> ContactSet
		{
			get
			{
				return this.CreateQuery<UpdateAnyEntity.Test.Contact>();
			}
		}
	}
}
