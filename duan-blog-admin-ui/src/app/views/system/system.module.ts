import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReactiveFormsModule } from '@angular/forms';
import { SystemRoutingModule } from './system-routing.module';
import { UserComponent } from './users/user.component';
import { RolesComponent } from './roles/roles.component';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { BlockUIModule } from 'primeng/blockui';
import { PaginatorModule } from 'primeng/paginator';
import { PanelModule } from 'primeng/panel';
import { CheckboxModule } from 'primeng/checkbox';
import { SharedModule } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { KeyFilterModule } from 'primeng/keyfilter';
import { RoleDetailComponent } from './roles/role-detail/role-detail.component';
import { DuanSharedModule } from '../../shared/modules/duan-shared.module';
import { PermissionGrantComponent } from './roles/permission-grant/permission-grant.component';
import { BadgeModule } from 'primeng/badge';
import { PickListModule } from 'primeng/picklist';
import { ImageModule } from 'primeng/image';
import { UserDetailComponent } from './users/user-detail/user-detail.component';
import { SetPasswordComponent } from './users/set-password/set-password.component';
import { RoleAssignComponent } from './users/role-assign/role-assign.component';
import { ChangeEmailComponent } from './users/change-email/change-email.component';

@NgModule({
  imports: [
    SystemRoutingModule,
    CommonModule,
    ReactiveFormsModule,
    TableModule,
    ProgressSpinnerModule,
    BlockUIModule,
    PaginatorModule,
    PanelModule,
    CheckboxModule,
    ButtonModule,
    InputTextModule,
    KeyFilterModule,
    SharedModule,
    ReactiveFormsModule,
    DuanSharedModule,
    BadgeModule,
    PickListModule,
    ImageModule,
  ],
  declarations: [
    UserComponent,
    RolesComponent,
    RoleDetailComponent,
    PermissionGrantComponent,
    UserDetailComponent,
    SetPasswordComponent,
    RoleAssignComponent,
    ChangeEmailComponent,
  ],
})
export class SystemModule {}
