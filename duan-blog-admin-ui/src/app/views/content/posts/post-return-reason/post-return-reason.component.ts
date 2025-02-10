import { Component, EventEmitter, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiPostApiClient } from '../../../../api/admin-api.service.generated';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-post-return-reason',
  templateUrl: './post-return-reason.component.html',
  styleUrls: ['./post-return-reason.component.scss'],
})
export class PostReturnReasonComponent implements OnInit, OnDestroy {
  //#region  variables
  private ngUnsubscribe = new Subject<void>();

  // Default
  public blockedPanelDetail: boolean = false;
  public form: FormGroup;
  public title: string;
  public btnDisabled = false;
  public saveBtnName: string;
  public contentTypes: any[] = [];

  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  validationMessages = {
    reason: [{ type: 'required', message: 'Bạn phải nhập lý do' }],
  };
  //#endregion
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private fb: FormBuilder,
    private postApiClient: AdminApiPostApiClient
  ) {}
  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  ngOnInit(): void {
    this.form = this.fb.group({
      reason: new FormControl(null, Validators.required),
    });
  }

  //#region  function
  saveChange() {
    this.toggleBlockUI(true);
    this.postApiClient
      .returnBack(this.config.data.id, this.form.value)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.ref.close(this.form.value);
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
      this.btnDisabled = true;
      this.blockedPanelDetail = true;
    } else {
      setTimeout(() => {
        this.btnDisabled = false;
        this.blockedPanelDetail = false;
      }, 1000);
    }
  }
  //#endregion
}
