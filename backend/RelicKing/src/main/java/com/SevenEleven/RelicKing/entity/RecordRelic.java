package com.SevenEleven.RelicKing.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Embeddable;
import lombok.AccessLevel;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Embeddable
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@ToString
public class RecordRelic implements Comparable<RecordRelic> {

	@Column(nullable = false)
	private int relicNo;

	@Column(nullable = false)
	private int level;

	@Column(nullable = false)
	private int slot;

	public void changeRelic(int relicNo, int level, int slot) {
		this.relicNo = relicNo;
		this.level = level;
		this.slot = slot;
	}

	@Override
	public int compareTo(RecordRelic recordRelic) {
		if (recordRelic.getSlot() < slot) {
			return 1;
		} else if (recordRelic.getSlot() > slot) {
			return -1;
		}
		return 0;
	}
}
