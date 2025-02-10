import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import {
  AdminApiSeriesApiClient,
  PostInListDtoPagedResult,
  SeriesInListDtoPagedResult,
  SeriesDto,
  SeriesInListDto,
} from '../../../api/admin-api.service.generated';
import { ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogComponent } from 'primeng/dynamicdialog';
import { AlertService } from '../../../shared/services/alert.service';
import { MessageConstants } from '../../../shared/constants/message-constant';
import { SeriesDetailComponent } from './series-detail/series-detail.component';

@Component({
  selector: 'app-series',
  templateUrl: './series.component.html',
  styleUrls: ['./series.component.scss'],
})
export class SeriesComponent implements OnInit, OnDestroy {
  //#region  variables

  private ngUnsubscribe = new Subject<void>();
  public blockedPanel: boolean = false;

  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalCount: number;

  public items: SeriesInListDto[];
  public selectedItems: SeriesInListDto[] = [];
  public keyword: string = '';

  //#endregion
  constructor(
    private seriesApiClient: AdminApiSeriesApiClient,
    public dialogService: DialogService,
    private notificationService: AlertService,
    private confirmationService: ConfirmationService
  ) {}
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  ngOnInit(): void {
    this.loadData();
  }

  loadData(selectionId = null) {
    this.toggleBlockUI(true);

    this.seriesApiClient
      .getSeriesPaging(this.keyword, this.pageIndex, this.pageSize)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: SeriesInListDtoPagedResult) => {
          this.items = response.results;
          this.totalCount = response.rowCount;
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  pageChanged(event: any): void {
    this.pageIndex = event.page;
    this.pageSize = event.rows;
    this.loadData();
  }

  showAddModal() {
    const ref = this.dialogService.open(SeriesDetailComponent, {
      header: 'Thêm mới series bài viết',
      width: '70%',
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: SeriesDto) => {
      if (data) {
        this.notificationService.showSuccess(MessageConstants.CREATED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
      }
    });
  }

  showEditModal() {
    if (this.selectedItems.length == 0) {
      this.notificationService.showError(
        MessageConstants.NOT_CHOOSE_ANY_RECORD
      );
      return;
    }
    var id = this.selectedItems[0].id;
    const ref = this.dialogService.open(SeriesDetailComponent, {
      data: {
        id: id,
      },
      header: 'Cập nhật series bài viết',
      width: '70%',
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: SeriesDto) => {
      if (data) {
        this.notificationService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
        this.loadData(data.id);
      }
    });
  }

  showPosts() {
    if (this.selectedItems.length == 0) {
      this.notificationService.showError(
        MessageConstants.NOT_CHOOSE_ANY_RECORD
      );
      return;
    }
    var id = this.selectedItems[0].id;
    const ref = this.dialogService.open(SeriesDetailComponent, {
      data: {
        id: id,
      },
      header: 'Quản lý danh sách bài viết',
      width: '70%',
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: SeriesDto) => {
      if (data) {
        this.notificationService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
        this.loadData(data.id);
      }
    });
  }

  //#region  function

  deleteItems() {
    if (this.selectedItems.length == 0) {
      this.notificationService.showError(
        MessageConstants.NOT_CHOOSE_ANY_RECORD
      );
      return;
    }
    var ids = [];
    this.selectedItems.forEach((element) => {
      ids.push(element.id);
    });
    this.confirmationService.confirm({
      message: MessageConstants.CONFIRM_DELETE_MSG,
      accept: () => {
        this.deleteItemsConfirm(ids);
      },
    });
  }

  deleteItemsConfirm(ids: any[]) {
    this.toggleBlockUI(true);

    this.seriesApiClient.deleteSeries(ids).subscribe({
      next: () => {
        this.notificationService.showSuccess(MessageConstants.DELETED_OK_MSG);
        this.loadData();
        this.selectedItems = [];
        this.toggleBlockUI(false);
      },
      error: () => {
        this.toggleBlockUI(false);
      },
    });
  }
  //#endregion

  //#region  private

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }
  //#endregion
}
