import { Component, EventEmitter, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import {
  AdminApiRoleApiClient,
  PermissionDto,
  RoleClaimsDto,
} from '../../../../api/admin-api.service.generated';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-permission-grant',
  templateUrl: './permission-grant.component.html',
  styleUrls: ['./permission-grant.component.scss'],
})
export class PermissionGrantComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  // biến
  public blockedPanelDetail = false;
  public form: FormGroup;
  public title: string;
  public btnDisabled = false;
  public saveBtnName: string;
  public closeBtnName: string;
  public permissions: RoleClaimsDto[] = [];
  public selectedPermissions: RoleClaimsDto[] = [];
  public id: string;
  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private roleService: AdminApiRoleApiClient,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.buildForm();
    this.loadDetail(this.config.data.id);
    this.saveBtnName = 'Cập nhật';
    this.closeBtnName = 'Hủy';
  }

  loadDetail(id: string): void {
    this.toggleBlockingPanel(true);
    this.roleService
      .getAllRolePermissions(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PermissionDto) => {
          this.permissions = response.roleClaims;
          this.buildForm();
          this.toggleBlockingPanel(false);
        },
        error: () => {
          this.toggleBlockingPanel(false);
        },
      });
  }

  buildForm(): void {
    this.form = this.fb.group({});

    for (let index = 0; index < this.permissions.length; index++) {
      const permission = this.permissions[index];
      if (permission.selected) {
        this.selectedPermissions.push(
          new RoleClaimsDto({
            selected: true,
            displayName: permission.displayName,
            type: permission.type,
            value: permission.value,
          })
        );
      }
    }
  }

  saveChange() {
    this.toggleBlockingPanel(true);
    this.saveData();
  }

  private saveData() {
    var roleClaims: RoleClaimsDto[] = [];
    for (let index = 0; index < this.permissions.length; index++) {
      const isGranted =
        this.selectedPermissions.filter(
          (x) => x.value == this.permissions[index].value
        ).length > 0;

      roleClaims.push(
        new RoleClaimsDto({
          type: this.permissions[index].type,
          selected: isGranted,
          value: this.permissions[index].value,
        })
      );
    }

    var updateValues = new PermissionDto({
      roleId: this.config.data.id,
      roleClaims: roleClaims,
    });

    this.roleService
      .updateRolePermissions(updateValues)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(() => {
        this.toggleBlockingPanel(false);
        this.ref.close(this.form.value);
      });
  }
  private toggleBlockingPanel(enable: boolean) {
    if (enable) {
      this.blockedPanelDetail = true;
    } else {
      setTimeout(() => {
        this.blockedPanelDetail = false;
      }, 1000);
    }
  }

  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
