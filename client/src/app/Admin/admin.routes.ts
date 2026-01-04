import { Routes } from "@angular/router";
import { AdminProductComponent } from "./admin-products/admin-product.component";
import { authGuard } from "../core/guards/auth.guard";
import { AdminCreateComponent } from "./AdminCreateProduct/admin-create.component";
import { AdminEditComponent } from "./AdminEditProduct/admin-edit.component";



export const AdminRoutes: Routes = [
  { path: 'products', component: AdminProductComponent, canActivate: [authGuard], title: 'AdminDashboard' },
  { path: 'products/create', component: AdminCreateComponent, canActivate: [authGuard], title: 'Create' },
  { path: 'products/edit/:id', component: AdminEditComponent, canActivate: [authGuard], title: 'Edit' },

]
