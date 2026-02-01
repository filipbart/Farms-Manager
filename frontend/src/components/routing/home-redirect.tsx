import { Navigate } from "react-router-dom";
import { usePermissions } from "../../context/permission-context";
import { useRouter } from "../../router/use-router";
import { RouteName } from "../../router/route-names";

/**
 * Component that redirects to appropriate home page based on user permissions.
 * If user has dashboard:view permission -> redirect to Dashboard
 * Otherwise -> redirect to Welcome page
 */
const HomeRedirect: React.FC = () => {
  const { hasPermission } = usePermissions();
  const { getRoute } = useRouter();

  if (!hasPermission("dashboard:view")) {
    return <Navigate to={getRoute(RouteName.Welcome)} replace />;
  }

  return <Navigate to={getRoute(RouteName.Dashboard)} replace />;
};

export default HomeRedirect;
