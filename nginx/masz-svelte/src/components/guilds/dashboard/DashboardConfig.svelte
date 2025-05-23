<script lang="ts">
    import { WidgetMode } from "./../../../core/dashboard/WidgetMode";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import {
        guildDashboardClearingCache,
        guildDashboardEnableDragging,
        guildDashboardItems,
        guildDashboardToggledItems,
        visibleGuildDashboardItems,
    } from "../../../stores/dashboardItems";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import { Modal } from "carbon-components-svelte";
    import { Toggle } from "carbon-components-svelte";
    import { CircleSolid, SettingsAdjust } from "carbon-icons-svelte";
    import { confirmDialogMessageKey, confirmDialogReturnFunction, showConfirmDialog } from "../../../core/confirmDialog/store";
    import API from "../../../services/api/api";
    import { toastInfo } from "../../../services/toast/store";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Normal;
    let localItems = [];
    let cacheClearToggle = false;

    const showModal: Writable<boolean> = writable(false);

    function openModal() {
        showModal.set(true);
        localItems = $guildDashboardItems;
        guildDashboardEnableDragging.set(false);
    }

    function onModalClose() {
        showModal.set(false);
    }

    function widgetIsActivated(item: IDashboardItem): boolean {
        return $guildDashboardToggledItems.length === 0 || $guildDashboardToggledItems.some((x) => x.id === item.id && x.enabled);
    }

    function widgetIsToggled(toggled: boolean, item: IDashboardItem) {
        guildDashboardToggledItems.update((n) => {
            const index = n.findIndex((x) => x.id === item.id);
            if (index === -1) {
                n.unshift({
                    id: item.id,
                    enabled: toggled,
                    sortOrder: -1,
                });
            } else {
                n[index].enabled = toggled;
            }
            return n;
        });
    }

    function onClearCacheToggle() {
        if (cacheClearToggle) {
            confirmDialogReturnFunction.set(onClearCacheConfirm);
            confirmDialogMessageKey.set("widgets.dashboardconfig.clearcacheconfirm");
            showConfirmDialog.set(true);
        }
    }

    function onClearCacheConfirm(confirmed) {
        cacheClearToggle = false;
        if (confirmed) {
            guildDashboardClearingCache.set(true);
            guildDashboardEnableDragging.set(true);
            const count = API.clearCache(true);
            toastInfo($_("widgets.dashboardconfig.clearcachesuccess", { values: { count } }));
            setTimeout(() => {
                guildDashboardClearingCache.set(false);
                guildDashboardEnableDragging.set(false);
            }, 200);
        }
    }
</script>

<DashboardWidget
    title={$_("widgets.dashboardconfig.title")}
    mode={dashboardItem.mode}
    state={widgetState}>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.dashboardconfig.available")}
        </div>
        <div class="mr-1">
            {$guildDashboardItems.filter((x) => !x.fix).length}
        </div>
        <CircleSolid style="fill: var(--cds-ui-04)" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.dashboardconfig.enabled")}
        </div>
        <div class="mr-1">
            {$visibleGuildDashboardItems.filter((x) => !x.fix).length}
        </div>
        <CircleSolid style="fill: var(--cds-ui-04)" />
    </div>
    <div
        class="dash-widget-list-border flex flex-row items-center py-2"
        class:cursor-pointer={!$guildDashboardEnableDragging}
        style="height: 2rem"
        on:click={openModal}>
        <div class="grow">
            {$_("widgets.dashboardconfig.configurewidgets")}
        </div>
        <SettingsAdjust />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.dashboardconfig.enabledragging")}
        </div>
        <div>
            <Toggle
                size="sm"
                labelText=""
                labelA=""
                labelB=""
                disabled={$guildDashboardClearingCache}
                toggled={$guildDashboardEnableDragging}
                on:toggle={(e) => guildDashboardEnableDragging.set(e.detail.toggled)} />
        </div>
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.dashboardconfig.clearcache")}
        </div>
        <div>
            <Toggle size="sm" labelText="" labelA="" labelB="" bind:toggled={cacheClearToggle} on:toggle={onClearCacheToggle} />
        </div>
    </div>
</DashboardWidget>

<Modal size="sm" open={$showModal} modalHeading="Widgets" passiveModal on:close={onModalClose} on:submit={onModalClose}>
    <div class="flex flex-col">
        {#each localItems as item (item.id)}
            <div class="flex flex-row border-2 border-bottom mb-4">
                <div class="flex justify-center align-center mr-10 shrink-0">
                    <Toggle
                        title={item.fix ? "This widget cannot be disabled." : ""}
                        disabled={item.fix}
                        labelText=""
                        toggled={item.fix ? true : widgetIsActivated(item)}
                        on:toggle={(e) => {
                            widgetIsToggled(e.detail.toggled, item);
                        }} />
                </div>
                <div class="flex flex-col grow">
                    <div>
                        {$_(`widgets.${item.translationKey}.title`)}
                    </div>
                    <div style="color: var(--cds-text-02)">
                        {$_(`widgets.${item.translationKey}.widgetdescription`)}
                    </div>
                </div>
            </div>
        {/each}
    </div>
</Modal>
