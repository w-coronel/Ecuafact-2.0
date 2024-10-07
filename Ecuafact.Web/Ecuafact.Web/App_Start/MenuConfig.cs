using EcuafactExpress.Web.Models;using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcuafactExpress.Web.App_Start
{
    class MenuConfig
    {
        internal static void RegisterMenus(NavMenuItemCollection Menu)
        {

            Menu.Add("Inicio", "~/", "icon-home");
            
            Menu.Add("Documentos");
            Menu.Add("Recibidos", "~/Comprobantes/Recibidos", "fa fa-inbox", badgeCount: ()=> SessionInfo.DocumentsReceived.Count(), role: UserRoleEnum.Issuer);
            Menu.Add("Emitidos", "~/Comprobantes/Emitidos", icon: "fa fa-paper-plane", badgeCount:()=> SessionInfo.DashboardInfo.DocumentsCount, badgeType: NavTypeEnum.Warning, role: UserRoleEnum.Issuer);
            Menu.Add("Borradores", "~/Comprobantes/Borradores", "fa fa-file", badgeCount: ()=>SessionInfo.DashboardInfo.DraftsCount, badgeType: NavTypeEnum.Danger, role: UserRoleEnum.Issuer);

            Menu.AddDivider();
            Menu.Add("Gastos Deducibles", "~/Gastos", "fa fa-money");
            Menu.Add("Presupuesto", "~/Presupuesto", "fa fa-usd");
            
            Menu.AddDivider();
            Menu.Add("Contribuyentes", "~/Contribuyentes", "fa fa-users", role: UserRoleEnum.Issuer);
            Menu.Add("Productos", "~/Producto", "fa fa-fax", role: UserRoleEnum.Issuer);
            Menu.AddDivider();

            Menu.Add("Configuración", "~/Config", icon: "fa fa-gear", badgeCount: ()=>((SessionInfo.Issuer == null) ? 0 : 3), badgeType: NavTypeEnum.Danger );




        }

    }

}