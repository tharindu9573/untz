import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { UntzEvent } from 'src/app/models/event';
import { EventService } from 'src/services/event.service';
import { EventConfirmationPopupComponent } from '../event-confirmation-popup/event-confirmation-popup.component';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.css']
})
export class EventFormComponent implements OnInit{
  public event!: UntzEvent;

  @Output() closePopup = new EventEmitter();

  editEvent: boolean = false;

  public eventForm! : FormGroup;
  public isEditButtonIsDisabled: boolean = false;
  
  constructor(private formBuilder: FormBuilder, private eventService: EventService, private toastrService: ToastrService, private modelService: NgbModal,private sanitizer: DomSanitizer){}

  ngOnInit(): void {
    this.initializeForm();
    if(this.editEvent && this.event){      
      this.eventForm.patchValue(this.event);
      this.event.tickets.forEach(element => {
        this.tickets.push(this.formBuilder.group({
          id: element.id,
          name: element.name,
          price: element.price
        }))  
      });
      this.event.images.forEach(async element=>{
        const response = await fetch(element.filePath);
        const blob = await response.blob();
        var match = /^data:(.*);base64,(.*)$/.exec(await this.blobToBase64(blob));
        this.images.push(this.formBuilder.group({
          id: element.id,
          name: element.name,
          base64Content: match![2]
        }))
      }) 
    }
  }

  async blobToBase64(blob: Blob): Promise<string> {
    return new Promise<string>((resolve, reject) => {
        const reader = new FileReader();
        reader.onloadend = () => {
            if (typeof reader.result === 'string') {
                resolve(reader.result);
            } else {
                reject(new Error('Failed to convert Blob to base64.'));
            }
        };
        reader.onerror = reject;
        reader.readAsDataURL(blob);
    });
  }
  
  initializeForm(){
    this.eventForm = this.formBuilder.group({
      id: 0,
      name: ['', Validators.required],
      description: ['', Validators.required],
      createdDate: ['', Validators.required],
      preSaleStartDate: ['', Validators.required],
      eventStartTime: ['', Validators.required],
      location: ['', Validators.required],
      entrance: ['', Validators.required],
      tickets: this.formBuilder.array([]),
      images: this.formBuilder.array([]),
      mainEvent: this.formBuilder.group({
        id: [this.event?.mainEvent.id ?? 0],
        isActive: [this.event?.mainEvent.isActive ?? false]
      })
    });  
  }

 onSubmit(){
    var confirmationModel = this.modelService.open(EventConfirmationPopupComponent, { backdrop: 'static' })
    confirmationModel.componentInstance.content = `Do you want to ${this.editEvent ? 'update' : 'add'} this event ?`;
    confirmationModel.componentInstance.command.subscribe((_: boolean) => {
      if(_){
        if(this.eventForm.valid){
          this.isEditButtonIsDisabled = true;
          if(this.editEvent){
            this.eventService.updateEvent(this.eventForm.value).subscribe(_ => {
              if(_.id){
                this.toastrService.success("Updated successfully")
                this.eventService.forceReload.next(true);
                this.closePopup.emit();
                this.eventForm.reset();                
              }
              this.isEditButtonIsDisabled = false;
            });
            
          }else{
            this.eventService.addEvent(this.eventForm.value).subscribe(_ => {
              if(_.id){
                this.toastrService.success("Inserted successfully")
                this.eventService.forceReload.next(true);
                this.closePopup.emit();
                this.eventForm.reset();                
              }    
              this.isEditButtonIsDisabled = false;          
            });            
          }
        }else{
          this.toastrService.warning("Invalid inputs found");
        }
      }
      confirmationModel.close();      
    })    
  }

  get tickets(): FormArray{
    return this.eventForm.get("tickets") as FormArray;
  }

  get images(): FormArray{
    return this.eventForm.get("images") as FormArray;
  }


  newTicket(): FormGroup{
    return this.formBuilder.group({
      id: 0,
      name: ['', Validators.required],
      price: [0, Validators.min(1)]
    });
  };

  newImage(image: any): FormGroup{
    return this.formBuilder.group({
      id: 0,
      name: image.name,
      base64Content: image.base64Content
    });
  };

  addTicket(){
    this.tickets.push(this.newTicket())
  }

  removeTicket(i: number){
    this.tickets.removeAt(i);
  }

  close(){
    this.closePopup.emit();
  }

  removeImage(i: number){
    this.images.removeAt(i);
  }

  getImage(content: any): any{
    if(content){
      const data = content;
      const imageDataUrl = 'data:image/jpeg;base64,' + data;
      return this.sanitizer.bypassSecurityTrustResourceUrl(imageDataUrl);
    }
    return '';
  }

  addImage(event: any) {
    if (event.target.files && event.target.files[0]) {
        const reader = new FileReader();
        reader.onload = (_event: any) => {
          var match = /^data:(.*);base64,(.*)$/.exec(_event.target.result);
          if (match == null || (event.srcElement.files[0].type !== "image/jpeg" && event.srcElement.files[0].type !== "image/png")) {
              this.toastrService.warning("Please add images only!");
              return;
          }
          var content = match[2];
          this.images.push(this.newImage({
            name:event.srcElement.files[0].name,
            base64Content: content
          }));
        };
        reader.readAsDataURL(event.target.files[0]);
    }
  }

}
