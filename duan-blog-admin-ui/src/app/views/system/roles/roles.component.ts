import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import {
  AdminApiRoleApiClient,
  RoleDto,
  RoleDtoPagedResult,
} from '../../../api/admin-api.service.generated';
import { DialogService } from 'primeng/dynamicdialog';
import { AlertService } from '../../../shared/services/alert.service';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.scss'],
})
export class RolesComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  public blockedPanel: boolean = false;

  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalCount: number | undefined = 0;

  public items: RoleDto[] | undefined;
  public selectedItems: RoleDto[] = [];
  public keyword: string = '';

  constructor(
    private roleService: AdminApiRoleApiClient,
    public dialogService: DialogService,
    private alertService: AlertService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.toggleBlockingPanel(true);
    this.roleService
      .getRolesPaging(this.keyword, this.pageIndex, this.pageSize)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: RoleDtoPagedResult) => {
          this.items = response.results;
          this.totalCount = response.rowCount;

          this.toggleBlockingPanel(false);
        },
        error: (e) => {
          this.toggleBlockingPanel(false);
        },
      });
  }

  pageChanged(event: any) {
    this.pageIndex = event.page;
    this.pageSize = event.rows;
    this.loadData();
  }

  private toggleBlockingPanel(enable: boolean) {
    if (enable) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }

  showPermissionModal(id: string, name: string) {}
  showEditModal() {}
  showAddModal() {}
  deleteItems() {}

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
