import { Component, AfterViewInit, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AlertifyService } from 'app/core/services/alertify.service';
import { LookUpService } from 'app/core/services/lookUp.service';
import { AuthService } from 'app/core/components/admin/login/services/auth.service';
import { environment } from 'environments/environment';
import { Product } from '../product/models/Product';
import { ProductService } from '../product/services/Product.service';
import { Store } from './models/store';
import { StoreService } from './services/store.service';
import { StoreDto } from './models/storeDto';
import { map, startWith } from 'rxjs/operators';
import { Observable } from 'rxjs/Observable';
import { LookUp } from 'app/core/models/lookUp';

declare var jQuery: any;

@Component({
	selector: 'app-store',
	templateUrl: './store.component.html',
	styleUrls: ['./store.component.scss']
})
export class StoreComponent implements AfterViewInit, OnInit {

	dataSource: MatTableDataSource<any>;
	@ViewChild(MatPaginator) paginator: MatPaginator;
	@ViewChild(MatSort) sort: MatSort;
	displayedColumns: string[] = [
		'productName',
		'stock',
		'isReady',
		'update',
		'delete'];

	storeList: Store[];
	storeDtoList: StoreDto[];
	store: Store = new Store();
	storeDto: StoreDto = new StoreDto();
	productList: Product[];
	storeAddForm: FormGroup;

	productControl = new FormControl();
	filteredOptions: Observable<Product[]>;
	storeId: number;


	constructor(private storeService: StoreService, private lookupService: LookUpService, private alertifyService: AlertifyService, private formBuilder: FormBuilder, private authService: AuthService, private productService: ProductService) { }

	ngAfterViewInit(): void {
		this.getStoreDtoList();
	}

	ngOnInit() {
		this.createStoreAddForm();
		this.getProductsList();
	}

		applyFilter(event: Event) {
		const filterValue = (event.target as HTMLInputElement).value;
		this.dataSource.filter = filterValue.trim().toLowerCase();

		if (this.dataSource.paginator) {
			this.dataSource.paginator.firstPage();
		}
	}

	displayFn(data: Product): string {
		return data && data.name ? data.name : '';
	  }
	
	  private _filter(name: string): Product[] {
		const filterValue = name.toLowerCase();
	
		return this.productList.filter(option => option.name.toLowerCase().includes(filterValue));
	  }

	  getProductsList(){
		this.productService.getProductList().subscribe(data => {
			this.productList = data;

			this.filteredOptions = this.productControl.valueChanges.pipe(
				startWith(''),
				map(value => {
				  const name = typeof value === 'string' ? value : value?.name;
				  return name ? this._filter(name as string) : this.productList.slice();
				}));
		});
	  }

	getStoreList() {
		this.storeService.getStoreList().subscribe(data => {
			this.storeList = data;
			this.dataSource = new MatTableDataSource(data);
			this.configDataTable();
		});
	}

	getStoreDtoList() {
		this.storeService.getStoreDtoList().subscribe(data => {
			this.storeDtoList = data;
			this.dataSource = new MatTableDataSource(data);
			this.configDataTable();
		});
	}


	save() {
		debugger;
		this.storeAddForm.controls.productId.setValue(this.productControl.value.id);
		// this.storeAddForm.controls.status.setValue(this.store.status);
		// this.storeAddForm.controls.createdUserId.setValue(this.store.createdUserId);
		// this.storeAddForm.controls.id.setValue(this.store.id);
		if (this.storeAddForm.valid) {
			this.store = Object.assign({}, this.storeAddForm.value)
			
			if (this.store.id == 0)
				this.addStore();
			else
				this.updateStore();
		}
	}

	addStore() {
		this.store.createdUserId = this.authService.getUserId();
		this.store.lastUpdatedUserId = this.authService.getUserId();
		this.storeService.addStore(this.store).subscribe(data => {
			this.getStoreDtoList();
			this.store = new Store();
			jQuery('#store').modal('hide');
			this.alertifyService.success(data);
			this.clearFormGroup(this.storeAddForm);
		})
	}

	updateStore() {
		debugger;
		this.store.lastUpdatedUserId = this.authService.getUserId();
		this.storeService.updateStore(this.store).subscribe(data => {
			var index = this.storeDtoList.findIndex(x => x.id == this.storeDto.id);
			this.storeDtoList[index] = this.storeDto;
			this.getStoreDtoList();
			this.dataSource = new MatTableDataSource(this.storeDtoList);
			this.configDataTable();
			this.store = new Store();
			jQuery('#store').modal('hide');
			this.alertifyService.success(data);
			this.clearFormGroup(this.storeAddForm);
		})
	}

	createStoreAddForm() {
		debugger;
		this.storeAddForm = this.formBuilder.group({
			id: [0],
			createdUserId: [0],
			lastUpdatedUserId: [0],
			status: [true],
			productId: [0, Validators.required],
			stock: [0, Validators.required],
			isReady: [false, Validators.required]
		})
	}

	deleteStore(storeId: number) {
		this.storeService.deleteStore(storeId).subscribe(data => {
			this.alertifyService.success(data.toString());
			this.storeDtoList = this.storeDtoList.filter(x => x.id != storeId);
			this.dataSource = new MatTableDataSource(this.storeDtoList);
			this.configDataTable();
		})
	}

	getStoreById(storeId: number) {
		this.clearFormGroup(this.storeAddForm);
		this.storeService.getStoreById(storeId).subscribe(data => {
			this.store = data;
			this.storeAddForm.patchValue(data);
			console.log(data,"adata");
			
		})
	}

	getStoreDtoById(storeId: number) {
		this.clearFormGroup(this.storeAddForm);
		this.storeService.getStoreDtoById(storeId).subscribe(data => {
			this.store =  {
				id : data.id,
				isDeleted : data.isDeleted,
				isReady : data.isReady,
				status: data.status,
				createdDate: data.createdDate,
				createdUserId: data.createdUserId,
				lastUpdatedDate: data.lastUpdatedDate,
				lastUpdatedUserId: data.lastUpdatedUserId,
				productId: data.productId,
				stock: data.stock
			};
			
			 // Set selected product in mat-autocomplete
			 const selectedProduct = this.productList.find(product => product.id === data.productId);
			 this.productControl.setValue(selectedProduct);

			 this.storeAddForm.patchValue(data);

		})
	}


	clearFormGroup(group: FormGroup) {

		group.markAsUntouched();
		group.reset();

		Object.keys(group.controls).forEach(key => {
			group.get(key).setErrors(null);
			if (key == 'id')
				group.get(key).setValue(0);
				if (key == 'createdUserId')
				group.get(key).setValue(0);
				if (key == 'lastUpdatedUserId')
				group.get(key).setValue(0);
				if (key == 'productId')
				group.get(key).setValue(0);
				if (key == 'stock')
				group.get(key).setValue(0);
				if (key == 'isReady')
				group.get(key).setValue(0);
		});
	}

	checkClaim(claim: string): boolean {
		return this.authService.claimGuard(claim)
	}

	configDataTable(): void {
		this.dataSource.paginator = this.paginator;
		this.dataSource.sort = this.sort;
	}

}
