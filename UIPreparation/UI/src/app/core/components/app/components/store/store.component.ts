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
	productList: Product[];
	storeAddForm: FormGroup;

	storeId: number;


	constructor(private storeService: StoreService, private lookupService: LookUpService, private alertifyService: AlertifyService, private formBuilder: FormBuilder, private authService: AuthService, private productService: ProductService) { }

	ngAfterViewInit(): void {
		this.getStoreDtoList();
		
	}

	ngOnInit() {
		this.getProducts();
		this.createStoreAddForm();
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
			console.log(data, "store data");
			
		});
	}


	save() {
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
			this.getStoreList();
			this.store = new Store();
			jQuery('#store').modal('hide');
			this.alertifyService.success(data);
			this.clearFormGroup(this.storeAddForm);

		})

	}

	updateStore() {
		this.store.lastUpdatedUserId = this.authService.getUserId();
		this.storeService.updateStore(this.store).subscribe(data => {

			var index = this.storeList.findIndex(x => x.id == this.store.id);
			this.storeList[index] = this.store;
			this.dataSource = new MatTableDataSource(this.storeList);
			this.configDataTable();
			this.store = new Store();
			jQuery('#store').modal('hide');
			this.alertifyService.success(data);
			this.clearFormGroup(this.storeAddForm);

		})

	}

	createStoreAddForm() {
		this.storeAddForm = this.formBuilder.group({
			id: [0],
			createdUserId: [0],
			createdDate: [Date.now],
			lastUpdatedUserId: [0],
			lastUpdatedDate: [Date.now],
			status: [true],
			isDeleted: [false],
			productId: [0, Validators.required],
			stock: [0, Validators.required],
			isReady: [false, Validators.required]
		})
	}

	deleteStore(storeId: number) {
		this.storeService.deleteStore(storeId).subscribe(data => {
			this.alertifyService.success(data.toString());
			this.storeList = this.storeList.filter(x => x.id != storeId);
			this.dataSource = new MatTableDataSource(this.storeList);
			this.configDataTable();
		})
	}

	getStoreById(storeId: number) {
		this.clearFormGroup(this.storeAddForm);
		this.storeService.getStoreById(storeId).subscribe(data => {
			this.store = data;
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
		});
	}

	checkClaim(claim: string): boolean {
		return this.authService.claimGuard(claim)
	}

	configDataTable(): void {
		this.dataSource.paginator = this.paginator;
		this.dataSource.sort = this.sort;
	}

	applyFilter(event: Event) {
		const filterValue = (event.target as HTMLInputElement).value;
		this.dataSource.filter = filterValue.trim().toLowerCase();

		if (this.dataSource.paginator) {
			this.dataSource.paginator.firstPage();
		}
	}

	getProducts() {
		this.productService.getProductList().subscribe(data => {
			this.productList = data;
		});
	}

}
