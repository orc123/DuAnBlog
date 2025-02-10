import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  AdminApiPostApiClient,
  AdminApiPostCategoryApiClient,
  AdminApiTestApiClient,
  PostCategoryDto,
  PostDto,
  PostInListDto,
  PostInListDtoPagedResult,
} from '../../../api/admin-api.service.generated';
import { Subject, takeUntil } from 'rxjs';
import { ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogComponent } from 'primeng/dynamicdialog';
import { AlertService } from '../../../shared/services/alert.service';
import { PostDetailComponent } from './post-detail/post-detail.component';
import { MessageConstants } from '../../../shared/constants/message-constant';
import { PostSeriesComponent } from './post-series/post-series.component';
import { PostReturnReasonComponent } from './post-return-reason/post-return-reason.component';
import { PostActivityLogsComponent } from './post-activity-logs/post-activity-logs.component';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
})
export class PostComponent implements OnInit, OnDestroy {
  //#region  variables
  blockedPanel: boolean = false;
  private ngUnsubscribe = new Subject<void>();

  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalCount: number;
  public first = 0;

  public items: PostInListDto[];
  public selectedItems: PostInListDto[] = [];
  public keyword: string = '';

  public categoryId?: string = null;
  public postCategories: any[] = [];

  //#endregion
  constructor(
    private postCategoryApiClient: AdminApiPostCategoryApiClient,
    private postApiClient: AdminApiPostApiClient,
    public dialogService: DialogService,
    private alertService: AlertService,
    private confirmationService: ConfirmationService
  ) {}
  ngOnInit(): void {
    this.first = (this.pageIndex - 1) * this.pageSize;
    this.loadPostCategories();
    this.loadData();
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  //#region  function

  loadData(selectionId = null) {
    this.toggleBlockUI(true);
    this.postApiClient
      .getPostsPaging(
        this.keyword,
        this.categoryId,
        this.pageIndex,
        this.pageSize
      )
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PostInListDtoPagedResult) => {
          this.items = response.results;
          this.totalCount = response.rowCount;
          this.toggleBlockUI(false);
        },
        error: () => this.toggleBlockUI(false),
      });
  }
  loadPostCategories() {
    this.postCategoryApiClient
      .getPostCategories()
      .subscribe((response: PostCategoryDto[]) => {
        response.forEach((element) => {
          this.postCategories.push({
            value: element.id,
            label: element.name,
          });
        });
      });
  }

  pageChanged(event: any) {
    this.pageIndex = event.page;
    this.pageSize = event.rows;
    this.loadData();
  }
  showAddModal() {
    const ref = this.dialogService.open(PostDetailComponent, {
      header: 'Thêm mới bài viết',
      width: '70%',
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: PostDto) => {
      if (data) {
        this.alertService.showSuccess(MessageConstants.CREATED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
      }
    });
  }
  showEditModal() {
    if (this.selectedItems.length == 0) {
      this.alertService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    var id = this.selectedItems[0].id;
    const ref = this.dialogService.open(PostDetailComponent, {
      data: {
        id: id,
      },
      header: 'Cập nhật bài viết',
      width: '70%',
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: PostDto) => {
      if (data) {
        this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
      }
    });
  }
  deleteItems() {
    if (this.selectedItems.length == 0) {
      this.alertService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    var ids = [];
    this.selectedItems.forEach((element) => {
      ids.push(element.id);
    });
    this.confirmationService.confirm({
      message: MessageConstants.CONFIRM_DELETE_MSG,
      accept: () => {
        this.toggleBlockUI(true);
        this.postApiClient.deletePost(ids).subscribe({
          next: () => {
            this.alertService.showSuccess(MessageConstants.DELETED_OK_MSG);
            this.loadData();
            this.selectedItems = [];
            this.toggleBlockUI(false);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
      },
    });
  }

  addToSeries(id: string) {
    const ref = this.dialogService.open(PostSeriesComponent, {
      data: {
        id: id,
      },
      header: 'Thêm vào loạt bài',
      width: '70%',
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: PostDto) => {
      if (data) {
        this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
        this.loadData(data.id);
      }
    });
  }
  approve(id: string) {
    this.toggleBlockUI(true);
    this.postApiClient.approvePost(id).subscribe({
      next: () => {
        this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.loadData();
        this.selectedItems = [];
        this.toggleBlockUI(false);
      },
      error: () => {
        this.toggleBlockUI(false);
      },
    });
  }

  sendToApprove(id: string) {
    this.toggleBlockUI(true);
    this.postApiClient.sendToApprove(id).subscribe({
      next: () => {
        this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.loadData();
        this.selectedItems = [];
        this.toggleBlockUI(false);
      },
      error: () => {
        this.toggleBlockUI(false);
      },
    });
  }

  reject(id: string) {
    const ref = this.dialogService.open(PostReturnReasonComponent, {
      data: {
        id: id,
      },
      header: 'Trả lại bài',
      width: '70%',
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: PostDto) => {
      if (data) {
        this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
        this.loadData(data.id);
      }
    });
  }

  showLogs(id: string) {
    const ref = this.dialogService.open(PostActivityLogsComponent, {
      data: {
        id: id,
      },
      header: 'Xem lịch sử',
      width: '70%',
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: PostDto) => {
      if (data) {
        this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
        this.loadData(data.id);
      }
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
