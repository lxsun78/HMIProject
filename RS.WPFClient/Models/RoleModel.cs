namespace RS.WPFClient.Models
{
    public class RoleModel : ModelBase
    {

        private string? name;
        /// <summary>
        /// 角色名称
        /// </summary>
        public string? Name
        {
            get { return name; }
            set
            {
                this.SetProperty(ref name, value);
            }
        }



        private string? description;
        /// <summary>
        /// 备注
        /// </summary>
        public string? Description
        {
            get { return description; }
            set
            {
                this.SetProperty(ref description, value);
            }
        }


        /// <summary>
        /// 绑定公司Id
        /// </summary>
        public long? CompanyId { get; set; }


        private string? companyName;
        /// <summary>
        /// 绑定公司名称
        /// </summary>
        public string? CompanyName
        {
            get { return companyName; }
            set
            {
                this.SetProperty(ref companyName, value);
            }
        }


    }
}
