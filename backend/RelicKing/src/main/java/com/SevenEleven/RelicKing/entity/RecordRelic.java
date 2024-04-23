package com.SevenEleven.RelicKing.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Entity
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor
@ToString
public class RecordRelic {

	@Id
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private int recordRelicId;

	@ManyToOne
	@JoinColumn(name = "record_id", referencedColumnName = "record_id", nullable = false)
	private Record record;

	@Column(nullable = false)
	private int relicNo;

	@Column(nullable = false)
	private int level;

	@Column(nullable = false)
	private int slot;
}
